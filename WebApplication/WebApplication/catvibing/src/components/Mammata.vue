<template>
  <v-container>
    <v-row class="text-center">
      <v-flex class="mb-4">
        <v-img
          :src="require('../assets/logo.svg')"
          class="my-3"
          contain
          height="200"
        />
        <h1 class="display-2 font-weight-bold mb-3">
          Welcome to Mammata
        </h1>

        <v-form ref="form" @submit.prevent="onClick" lazy-validation v-model="valid">
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="cattext"
            label="Cat text"
            v-model="catText"
          />
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="drummertext"
            label="Drummer text"
            v-model="drummerText"
          />
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="drumtext"
            label="Drum text"
            v-model="drumText"
          />
          <v-btn color="primary" type="submit">Carchimi</v-btn>
        </v-form>
      </v-flex>

      <v-col class="mb-4">
        <template v-for="(meme, index) in memes">
          <Meme :Guid="meme" :key="index" @deleteMe="deleteMeme"/>
        </template>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue, Watch } from "vue-property-decorator";
import axios from "axios";
import Meme from "@/components/Meme.vue";

@Component({ components: { Meme } })
export default class Mammata extends Vue {
  private catText = "";
  private drummerText = "";
  private drumText = "";
  private valid = false;
  private memes = new Array<string>();
  private requireRule = [(v: any) => !!v || "This field is required"];

  constructor() {
    super();
    if(localStorage["memes"] === undefined){
      localStorage["memes"] = "[]";
    }
    this.memes = JSON.parse(localStorage["memes"]);
  }
  
  onClick(): void {
    if (this.valid) {
      const MemeInfo: object = {
        catText: this.catText,
        drummerText: this.drummerText,
        drumText: this.drumText
      };
      axios.post("/meme/create", MemeInfo).then(x => {
        const guid: string = x.data.split(":")[0];
        if(this.memes.includes(guid))
          return;
        this.memes.push(guid);
      });
    }
  }

  @Watch("memes")
  onMemesChange(val: Array<string>, newVal: Array<string>){
    localStorage["memes"] = JSON.stringify(newVal);
  }

  deleteMeme(e: string){
    this.$delete(this.memes, this.memes.indexOf(e));
  }
}
</script>
