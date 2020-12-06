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
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="cattext"
            label="Cat text"
            v-model="catText"
          ></v-text-field>
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="drummertext"
            label="Drummer text"
            v-model="drummerText"
          ></v-text-field>
          <v-text-field
            :rules="requireRule"
            required
            filled
            id="drumtext"
            label="Drum text"
            v-model="drumText"
          ></v-text-field>
          <v-btn color="primary" @click="onClick">Carchimi</v-btn>
        </v-form>
      </v-col>

      <v-col class="mb-4">
        <template v-for="(meme, index) in memes">
          <Meme :guid="(meme)" :key="index" />
        </template>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { Component, Vue } from "vue-property-decorator";
import axios from "axios";
import Meme from "@/components/Meme.vue";

@Component({ components: { Meme } })
export default class Mammata extends Vue {
  private catText: string | undefined;
  private drummerText: string | undefined;
  private drumText: string | undefined;
  private valid = false;
  private memes: Array<string> = new Array<string>();
  private requireRule: Array<object> = [
    (v: any) => !!v || "This field is required"
  ];

  onClick(): void {
    this.$refs.form.validate();
    if (this.valid) {
      const MemeInfo: object = {
        catText: this.catText,
        drummerText: this.drummerText,
        drumText: this.drumText
      };
      axios.post("http://127.0.0.1:5000/meme/create", MemeInfo).then(x => {
        const guid: string = x.data.split(":")[0];
        this.memes.push(guid);
      });
    }
  }
}
</script>
