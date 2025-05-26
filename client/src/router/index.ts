import { createRouter, createWebHistory } from 'vue-router'
import ChatArea from '@/views/ChatArea.vue'
import DirectMessage from '@/views/DirectMessage.vue'
import ServerSettings from '@/views/ServerSettings.vue'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
    path: '/',
    redirect: '/channels/main/general'
  },
  {
    path: '/channels/:serverId/:channelId',
    name: 'Channel',
    component: ChatArea,
    props: true
  },
  {
    path: '/dm/:userId',
    name: 'DirectMessage',
    component: DirectMessage,
    props: true
  },
  {
    path: '/dm',
    name: 'DirectMessages',
    component: DirectMessage
  },
  {
    path: '/settings/:serverId',
    name: 'ServerSettings',
    component: ServerSettings,
    props: true
  }
  ]
})

export default router
