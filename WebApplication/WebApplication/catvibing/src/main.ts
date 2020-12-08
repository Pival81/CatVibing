import Vue from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import vuetify from "./plugins/vuetify";
import axios from "axios";
import VuePlyr from "vue-plyr";
import "vue-plyr/dist/vue-plyr.css";

Vue.config.productionTip = false;

Vue.use(VuePlyr, {
  plyr: { storage: {
    enabled: false
    }}
});

axios.defaults.headers.common["crossdomain"] = true;

new Vue({
  vuetify,
  render: h => h(App)
}).$mount("#app");
