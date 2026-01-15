import { createRouter, createWebHistory } from "vue-router";
import LandingView from "@/views/LandingView.vue";
import LoginView from "@/views/Login.vue";
import MainLayout from "@/views/MainLayout.vue";
import HomeView from "@/views/HomeView.vue";
import Register from "@/views/Register.vue";
import TestChat from "@/views/TestChat.vue";
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "test-chat",
      component: TestChat,
    },
    {
      path: "/login",
      name: "login",
      component: LoginView,
    },
    {
      path: "/register",
      name: "register",
      component: Register,
    },
    {
      path: "/app",
      component: MainLayout,
      children: [
        {
          path: "",
          name: "home",
          component: HomeView,
        },
      ],
    },
    {
      path: "/:pathMatch(.*)*",
      redirect: "/",
    },
  ],
});

export default router;
