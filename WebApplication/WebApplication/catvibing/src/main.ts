import Vue from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import vuetify from "./plugins/vuetify";
import axios from "axios";
import VueResizeText from 'vue-resize-text';

Vue.config.productionTip = false;

axios.defaults.headers.common['crossdomain'] = true;
Vue.use(VueResizeText);

new Vue({
  vuetify,
  render: h => h(App)
}).$mount("#app");
