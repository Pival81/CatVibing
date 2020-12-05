<template>
  <v-app>
    <v-main>
      <Mammata />
    </v-main>
  </v-app>
</template>

<script lang="ts">
import Vue from "vue";
import Mammata from "./components/Mammata.vue";

export default Vue.extend({
  name: "App",

  components: {
    Mammata
  },

  data: () => ({
    //
  })
});
</script>
