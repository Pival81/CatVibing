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
      Drum text: {{ DrumText }}<br/>
      <v-chip>{{ Status }}</v-chip>
      <v-progress-circular :value="(Percentage)" color="randomcolor" v-show="Percentage > 0">
        <span class="percentage">{{ Percentage }}</span>
      </v-progress-circular>
    </v-card-text>
    <v-card-actions>
      <v-btn value="Delete" @click="onDelete" fab small elevation="2" dark color="error">
        <v-icon dark>mdi-delete</v-icon>
      </v-btn>
      <v-btn value="Watch" @click="dialog = true;" fab small elevation="2" dark>
        <v-icon dark>mdi-play</v-icon>
      </v-btn>
    </v-card-actions>
    <v-dialog v-model="dialog">
      <video-player>
        <source :src="`http://127.0.0.1:5000/meme/watch/{Guid}`"/>
      </video-player>
    </v-dialog>
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import axios from "axios";
import randomColor from "randomcolor";
import { videoPlayer } from 'vue-md-player'
import 'vue-md-player/dist/vue-md-player.css'

enum Status {
  Error = "Error",
  Stopped = "Stopped",
  Working = "Working",
  Done = "Done",
  Scheduled = "Scheduled"
}

@Component
export default class Meme extends Vue {
  public name = "Meme";
  private disabled = true;
  private loading = true;
  private dialog = false;
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
      this.Connection.onerror = (ev: Event) => {
        this.Status = Status.Error;
      };
    }
    this.loading = false;
    this.disabled = false;
  }

  onDelete(){
    axios.get(
      `http://localhost:5000/meme/delete/${this.Guid}`
    );
    this.$emit("deleteMe", this.Guid);
  }
}
</script>

<style scoped>
.percentage{
  font-size: 0.9em;
}
</style>
