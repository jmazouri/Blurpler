<template>
  <div id="app">
    <div class="errorbox" v-if="error">
        Error: {{error}}
    </div>

    <h1>
        <span class="blurple">Blu</span> <span class="dark-blurple">rple</span> <span class="greyple">r</span>
    </h1>

    <a class="submit dcbutton withAnim" v-show="previewImg" @click="blurple()">
        Blurple!
    </a>

    <div class="separator">
        🡺
    </div>

    <div class="previewImg">
        <img v-if="previewImg" :src="previewImg">
        <label class="fileInput" v-else>
            <img src="../public/upload.svg" :class="{'animated': isDropping}">
            <input @change="onFileChange" type="file" accept="image/*" />
            Click or Drag
        </label>
    </div>

    <div class="outputImg">
        <img :src="outputImg">
    </div>

    <div style="clear: both;"></div>

    <a class="dcbutton expander" @click.capture="showAdvanced = true" :class="{'shown' : showAdvanced, 'withAnim': !showAdvanced}">
        <label title="The threshold of color gradation. Play around until it looks good!">
            Threshold
            <input type="number" min="0" max="10" step="0.25" v-model="threshold" />
        </label>

        <label title="Amount of preblur to apply, for stylization. 0 is bad!">
            Preblur
            <input type="number" min="0" max="10" step="0.25" v-model="preblur" />
        </label>

        <label title="Whether to keep the transparent pixels in the source image, or overwrite them.">
            Keep Transparency?
            <input type="checkbox" v-model="keepTransparency" />
        </label>

        <label @click="showAdvanced = false">
            X
        </label>
    </a>

    <a class="note" v-if="outputImg" href="/">Try a different image</a>

  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import HelloWorld from './components/HelloWorld.vue';

@Component({
  components: {
    HelloWorld,
  },
})
export default class App extends Vue
{
    error: string | null = null;

    showAdvanced: boolean = false;
    isDropping: boolean = false;

    previewImg: string = "";
    outputImg: string = "";
    
    threshold: number = 3;
    keepTransparency: boolean = true;
    preblur: number = 0.001;

    get previewRawBase64(): string
    {
        return this.previewImg.substring(this.previewImg.indexOf(",") + 1);
    }

    mounted()
    {
        var self = this;

        window.addEventListener("dragover", function (e: DragEvent)
        {
            e.preventDefault();
            self.isDropping = true;
        });

        window.addEventListener("dragleave", function (e: DragEvent)
        {
            e.preventDefault();
            self.isDropping = false;
        });

        window.addEventListener("drop", function (e: DragEvent)
        {
            e.preventDefault();

            let file = e.dataTransfer.files[0];
            self.selectFile(file);

            self.isDropping = false;
        });
    }

    selectFile(file: File)
    {
        var image = new Image();
        var self = this;

        if (file.size > 5000000)
        {
            self.error = "File's too big - must be under 5mb";
            return;
        }

        image.onload = function()
        {
            self.loadFile(file);
            image.remove();
            self.error = "";
        };

        image.onerror = function()
        {
            self.error = "That's not a valid image";
            image.remove();
        }

        console.log(file);

        image.src = URL.createObjectURL(file);
    }

    loadFile(file: File)
    {
        var reader = new FileReader();
        var self = this;

        reader.onload = (e: Event) =>
        {
            self.previewImg = (<FileReader>e.target).result;
        };

        reader.readAsDataURL(file);
    }

    onFileChange(e: Event)
    {
        var target = (<HTMLInputElement>e.target);
        var files = target.files;

        if (!files || !files.length)
        {
            return;
        }

        this.selectFile(files[0]);
    }

    blurple()
    {
        let self = this;

        fetch("https://blurpler.azurewebsites.net/api/ImageTransformer",
        {
            headers:
            {
                'Accept': 'image/png',
                'Content-Type': 'application/json'
            },
            method: "POST",
            body: JSON.stringify(
            {
                Threshold: self.threshold, 
                KeepTransparency: self.keepTransparency, 
                Preblur: self.preblur, 
                ImageBase64: self.previewRawBase64
            })
        })
        .then(async function(res)
        {
            if (!res.ok)
            {
                self.error = await res.text();
                return;
            }

            var reader = new FileReader();

            reader.onload = (e: Event) =>
            {
                self.outputImg = (<FileReader>e.target).result;
            };

            let result = await res.blob();

            if (result)
            {
                reader.readAsDataURL(result);
            }
        })
        .catch(function(res)
        {
            self.error = res;
        });
    }
}
</script>

<style lang="scss">

.errorbox
{
    border: 1px solid rgba(240,71,71,.3);

    color: #f04747;
    text-align: center;

    margin-top: 0.25em;
    padding: 0.25em;
}

a
{
    color: #7289DA;
    text-decoration: none;
}

.note
{
    font-size: 0.8em;
    text-align: center;
    color: #7289DA;

    display: block;
    width: 100%;

    margin-top: 1em;
}

.previewImg, .outputImg
{
    display: inline-block;
    background: #2C2F33;

    width: 45%;
    min-height: 133px;

    line-height: 0;

    border-radius: 15px;
    overflow: hidden;

    img
    {
        width: 100%;
    }
}

.separator
{
    width: 0;
    height: 0;

    line-height: 133px;

    position: relative;

    left: 48%;
    
    font-size: 2em;
}

.outputImg
{
    float: right;
}

.dark-blurple
{
    color: #4e5d94;
}

.blurple
{
    color: #7289DA;
}

.greyple
{
    color: #99AAB5;
}

.dcbutton
{
    cursor: pointer;

    display: block;

    font-size: 2em;

    text-align: center;
    background-color: #7289da;
    border-radius: 3px;
    color: #fff;
    font-size: 15px;

    width: 250px;
    height: 45px;
    line-height: 45px;

    box-shadow: 0 2px 6px 0 rgba(0,0,0,.2);

    transition: box-shadow, transform 0.3s, 0.1s cubic-bezier(0.25, 0.25, 0.315, 1.35);

    &.withAnim
    {
        &:hover
        {
            box-shadow: none;
            transform: translateY(1px);
            cursor: pointer;
        }

        &:active
        {
            transform:scale(1.05);
        }
    }
}

.submit
{
    margin: 0 auto 3em auto;
}

@keyframes bounce
{
    0%
    {
        transform: scale(1);
    }
    50%
    {
        transform: scale(1.25);
    }
    100%
    {
        transform: scale(1);
    }
}

.fileInput
{
    display: block;
    cursor: pointer;

    width: 100%;
    height: 133px;

    text-align: center;

    img
    {
        height: 83%;

        &.animated
        {
            animation: bounce 0.5s ease-in-out;
            animation-iteration-count: infinite;
        }
    }
}

input[type=file]
{
    width: 0.1px;
	height: 0.1px;
	opacity: 0;
	overflow: hidden;
	position: absolute;
	z-index: -1;
}

body
{
    background: #23272A;
    color: rgba(255, 255, 255, 0.701961);
    font-family: "Open Sans", "Helvetica Neue", Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;

    margin: 0;
    padding: 0;
}

h1
{
    text-align: center;
    margin: 0.33em auto;
    font-size: 6em;
}

#app
{
    width: 85%;
    max-width: 1280px;

    margin: 0 auto;

    min-height: calc(100vh - 112px);
}

footer
{
    margin-top: 2.5em;

    box-sizing: border-box;

    font-size: 12px;
    text-align: center;
    width: 100%;
    height: 50px;

    padding-top: 0.5em;
    line-height: 1.5em;

    background-color: rgba(0, 0, 0, 0.66);
}

$expander-transition: 0.5s cubic-bezier(0.25, 0.25, 0.315, 1);

.expander
{
    background: #4e5d94;
    
    transition: all $expander-transition;
    transition-property: width, height;

    margin: 2em auto 0 auto;
    overflow: hidden;

    label
    {
        display: inline-block;
        width: 30%;

        pointer-events: visible;

        &:last-child
        {
            cursor: pointer;

            background: rgb(240, 71, 71);

            @media screen and (min-width: 601px)
            {
                width: 10%;
            }
        }

        @media screen and (max-width: 600px)
        {
            width: 50%;
        }
    }

    &::before
    {
        display: block;
        width: 100%;
        text-align: center;
        line-height: 0;
        
        position: relative;
        top: 50%;

        content: "Advanced Options";
    }

    & > *
    {
        transition: opacity $expander-transition;
        transition-duration: 0.2s;

        opacity: 0;
        pointer-events: none;
    }

    &.shown
    {
        &::before
        {
            content: none;
        }

        width: 100%;
        
        @media screen and (max-width: 600px)
        {
            height: 90px;
        }
        
        & > *
        {
            transition-delay: 0.1s;
            opacity: 1;
            pointer-events: initial;
        }
    }
}

</style>
