<template>
  <div id="app">
    <h1>
        <span class="blurple">Blu</span> <span class="dark-blurple">rple</span> <span class="greyple">r</span>
    </h1>

    <a class="submit dcbutton" v-show="previewImg" @click="blurple()">
        Blurple!
    </a>

    <div class="separator">
        🡺
    </div>

    <div class="previewImg">
        <img v-if="previewImg" :src="previewImg">
        <label class="fileInput" v-else>
            <img src="../public/upload.svg">
            <input @change="onFileChange" type="file" />
        </label>
    </div>

    <div class="outputImg">
        <img :src="outputImg">
    </div>

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
    previewImg: string = "";
    outputImg: string = "";
    
    threshold: number = 3;
    keepTransparency: boolean = true;
    preblur: number = 0.001;

    get previewRawBase64(): string
    {
        return this.previewImg.substring(this.previewImg.indexOf(",") + 1);
    }

    onFileChange(e: Event)
    {
        var target = (<HTMLInputElement>e.target);
        var files = target.files;

        if (!files || !files.length)
        {
            return;
        }

        var reader = new FileReader();
        var self = this;

        reader.onload = (e: Event) =>
        {
            self.previewImg = (<FileReader>e.target).result;
        };

        reader.readAsDataURL(files[0]);
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
            body: JSON.stringify({Threshold: 3, KeepTransparency: true, Preblur: 0.001, ImageBase64: this.previewRawBase64})
        })
        .then(async function(res)
        {
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
        .catch(function(res){ console.log(res) });
    }
}
</script>

<style lang="scss">

.previewImg, .outputImg
{
    display: inline-block;
    background: #2C2F33;

    width: 45%;
    min-height: 133px;

    img
    {
        width: 100%;
    }
}

.separator
{
    width: 0;
    height: 0;
    line-height: 0;

    position: relative;

    left: 48%;
    top: 20%;
    
    font-size: 2em;

    transform: translateY(-25%);
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

.submit
{
    margin: 0 auto 3em auto;
}

.fileInput
{
    display: block;
    cursor: pointer;

    width: 100%;
    height: 133px;

    img
    {
        height: 100%;
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
    margin-top: 60px;
    font-size: 6em;

    margin-bottom: 0.5em;
}

#app
{
    width: 85%;
    max-width: 1280px;

    height: 300px;

    margin: 0 auto;
}

footer
{
    box-sizing: border-box;

    position: absolute;

    bottom: 0%;
    font-size: 12px;
    text-align: center;
    width: 100%;

    padding: 1em;
}
</style>
