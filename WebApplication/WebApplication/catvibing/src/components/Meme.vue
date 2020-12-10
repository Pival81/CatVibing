<template>
  <v-card
    ref="card"
    shaped
    elevation="5"
    :disabled="disabled"
    :loading="loading"
  >
    <v-card-text>
      <v-simple-table>
        <template class="text-h3">
          <tr>
            <th>Cat text</th>
            <th>Drummer text</th>
            <th>Drum text</th>
          </tr>
          <tr>
            <td>{{ CatText }}</td>
            <td>{{ DrummerText }}</td>
            <td>{{ DrumText }}</td>
          </tr>
        </template>
      </v-simple-table>
      <br/>
      <div class="justify-space-around d-flex align-center">
        <v-chip :color="getStatusColor()">{{ Status }}</v-chip>
        <v-progress-circular rotate="270" :value="(Percentage)" :color="Color" v-show="Percentage > 0">
          <span class="percentage">{{ Percentage }}</span>
        </v-progress-circular>
        <v-btn value="Delete" @click="deleteMe" fab small elevation="2" dark color="error">
          <v-icon dark>mdi-delete</v-icon>
        </v-btn>
        <v-btn value="Watch" @click="dialog = true" fab small elevation="2" :dark="isDone()" :disabled="!isDone()">
          <v-icon dark>mdi-play</v-icon>
        </v-btn>
      </div>
    </v-card-text>
    <v-dialog v-model="dialog" @click:outside="closeVideo()">
        <vue-plyr style="max-width: 75vw" ref="plyr">
          <video max-width="75vw"
          controls
          crossorigin
          playsinline>
            <source :src="`/meme/watch/${this.Guid}`"
            type="video/mp4"/>
          </video>
        </vue-plyr>
    </v-dialog>
  </v-card>
</template>

<script lang="ts">
import { Component, Prop, Vue } from "vue-property-decorator";
import axios from "axios";
import randomColor from "randomcolor";

enum StatusType {
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
  public Status: StatusType = StatusType.Scheduled;
  public Percentage: number;
  @Prop({ type: String, required: true })
  public Guid!: string;
  private Connection!: WebSocket;
  private Color: string;

  constructor() {
    super();
    this.Percentage = -1;
    this.Color = randomColor({ luminosity:'dark' });
  }

  isDone(): boolean{
    return this.Status == StatusType.Done;
  }

  closeVideo(){
    this.$refs.plyr.player.stop();
  }

  getStatusColor(): string{
    switch (this.Status){
      case StatusType.Done:
        return "green";

      case StatusType.Error:
        return "red";

      case StatusType.Working:
        return "light-blue";

      case StatusType.Scheduled:
        return "lime";

      case StatusType.Stopped:
        return "deep-orange";
    }
  }

  async created() {
    const memeData = await axios.get(
      `/meme/get/${this.Guid}`
    ).catch(error => {
      console.log(error.response);
      if (error.response.status === 404){
        this.deleteMe();
        return;
      }
    });
    this.CatText = memeData.data.catText;
    this.DrummerText = memeData.data.drummerText;
    this.DrumText = memeData.data.drumText;
    this.Status = memeData.data.memeWork.status;
    this.Percentage = memeData.data.memeWork.percentage;
    if (this.Status != StatusType.Done) {
      this.Connection = new WebSocket(`ws://${document.location.host}:8181/${this.Guid}`);
      this.Connection.onmessage = (ev: MessageEvent) => {
        const num: number = parseInt(ev.data);
        if (!isNaN(num)) {
          this.Percentage = num;
        } else if (ev.data === "DONE\n") {
          this.Status = StatusType.Done;
          this.Connection.close();
        }
      };
      this.Connection.onerror = (ev: Event) => {
        this.Status = StatusType.Error;
      };
    }
    this.loading = false;
    this.disabled = false;
  }

  deleteMe(){
    axios.get(
      `/meme/delete/${this.Guid}`
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
