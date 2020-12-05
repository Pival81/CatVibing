<template>
  <v-container>
    <v-row class="text-center">
      <v-col class="mb-4">
        <v-img
          :src="require('../assets/logo.svg')"
          class="my-3"
          contain
          height="200"
        />
        <h1 class="display-2 font-weight-bold mb-3">
          Welcome to Mammata
        </h1>

        <v-form ref="form" v-model="valid">
          <v-text-field :rules="requireRule" required filled label="Cat text" v-model="catText"></v-text-field>
          <v-text-field :rules="requireRule" required filled label="Drummer text" v-model="drummerText"></v-text-field>
          <v-text-field :rules="requireRule" required filled label="Drum text" v-model="drumText"></v-text-field>
          <v-btn @click="onClick">Carchimi</v-btn>
        </v-form>
      </v-col>

      <v-col class="mb-4">

      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import axios from "axios";

@Component
export default class Mammataa extends Vue{
  private catText: string = null;
  private drummerText: string = null;
  private drumText: string = null;
  private valid: boolean = null;
  private memes: Array<Meme>;
  private requireRule: Array = [ v => !!v || "This field is requried" ]

  onClick() : void {
    this.$refs.form.validate();
    if(this.valid){
      const MemeInfo: object = {
        "catText": this.catText,
        "drummerText": this.drummerText,
        "drumText": this.drumText
      };
      axios.post("http://127.0.0.1:5000/meme/create", MemeInfo)
      .then(x => { console.log(x); })
    }
  }
}
</script>
