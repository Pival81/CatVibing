<template>
  <v-card ref="card" shaped elevation="5" disabled="disabled" loading="loading">
    <v-card-text>
      Cat text: {{ CatText }}<br />
      Drummer text: {{ DrummerText }}<br />
      Drum text: {{ DrumText }}
    </v-card-text>
    <v-chip>{{ Status }}</v-chip>
    <v-progress-circular value="Percentage" v-if="Percentage > 0" />
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import axios from "axios";

enum Status {
  Stopped,
  Working,
  Done,
  Scheduled
}

@Component
export default class Meme extends Vue {
  public name = "Meme";
  private disabled = true;
  private loading = true;
  public CatText!: string;
  public DrummerText!: string;
  public DrumText!: string;
  public Status!: Status;
  public Percentage = -1;
  @Prop({type: String, required: true})
  public Guid!: string;
  private Connection!: WebSocket;

  async created() {
    const memeData = (await axios.get(`https://localhost:5001/meme/get/${this.Guid}`)).data;
    this.CatText = memeData.catText;
    this.DrummerText = memeData.drummerText;
    this.DrumText = memeData.drumText;
    this.Status = memeData.memeWork.status;
    this.Percentage = memeData.memeWork.percentage;
    if(this.Status != Status.Done){
      this.Connection = new WebSocket(`ws://127.0.0.1:8181/${this.Guid}`);
      this.Connection.onmessage =(ev: MessageEvent) => {
        console.log(ev.data);
        const num: number = parseInt(ev.data);
        if (!isNaN(num)) {
          this.Percentage = num;
        } else if (ev.data == "DONE") {
          this.Status = Status.Done;
          this.Connection.close();
        }
      };
    }
    this.loading = false;
    this.disabled = false;
  }
}
</script>

<style scoped></style>
