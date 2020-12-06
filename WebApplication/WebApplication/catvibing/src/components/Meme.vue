<template>
  <v-card
    ref="card"
    shaped
    elevation="5"
    :disabled="disabled"
    :loading="loading"
  >
    <v-card-text>
      Cat text: {{ CatText }}<br />
      Drummer text: {{ DrummerText }}<br />
      Drum text: {{ DrumText }}
    </v-card-text>
    <v-chip>{{ Status }}</v-chip>
    <v-progress-circular :value="(Percentage)" color="randomcolor()" v-show="Percentage > 0">
      <span class="percentage">{{ Percentage }}</span>
    </v-progress-circular>
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import axios from "axios";
import ResizeText from 'vue-resize-text';
import randomColor from "randomcolor";

enum Status {
  Stopped = "Stopped",
  Working = "Working",
  Done = "Done",
  Scheduled = "Scheduled"
}

@Component
export default class Meme extends Vue {
  public name = "Meme";
  directives = { ResizeText }
  private disabled = true;
  private loading = true;
  public CatText = "";
  public DrummerText = "";
  public DrumText = "";
  public Status: Status = Status.Scheduled;
  public Percentage: number;
  @Prop({ type: String, required: true })
  public Guid!: string;
  private Connection!: WebSocket;

  constructor() {
    super();
    this.Percentage = -1;
  }

  randomcolor(): string{ return randomColor(); }

  async created() {
    const memeData = await axios.get(
      `http://localhost:5000/meme/get/${this.Guid}`
    );
    this.CatText = memeData.data.catText;
    this.DrummerText = memeData.data.drummerText;
    this.DrumText = memeData.data.drumText;
    this.Status = memeData.data.memeWork.status;
    this.Percentage = memeData.data.memeWork.percentage;
    if (this.Status != Status.Done) {
      this.Connection = new WebSocket(`ws://127.0.0.1:8181/${this.Guid}`);
      this.Connection.onmessage = (ev: MessageEvent) => {
        const num: number = parseInt(ev.data);
        if (!isNaN(num)) {
          this.Percentage = num;
        } else if (ev.data === "DONE\n") {
          this.Status = Status.Done;
          this.Connection.close();
        }
      };
      this.Connection.onerror = (ev: ErrorEvent) => {
        ;
      };
    }
    this.loading = false;
    this.disabled = false;
  }
}
</script>

<style scoped>
.percentage{
  font-size: 0.9em;
}
</style>
