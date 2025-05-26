<template>
  <div class="app-container">
    <!-- Sidebar luôn hiển thị -->
    <Sidebar 
      @selectChannel="handleChannelSelect"
      @selectDM="handleDMSelect"
      @selectServer="handleServerSelect"
    />
    
    <!-- Main content area với router -->
    <div class="main-content">
      <RouterView 
        :activeChannel="activeChannel"
        :activeServer="activeServer"
        :activeDM="activeDM"
      />
    </div>
    
    <!-- Friends list chỉ hiển thị khi ở tab messages -->
    <FriendsList 
      v-if="showFriendsList" 
      :friends="friends"
      @startDM="handleStartDM"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import { useRoute } from 'vue-router'
import Sidebar from '@/components/Sidebar/index.vue'
import FriendsList from '@/components/FriendsList.vue'

// State management
const activeChannel = ref('general')
const activeServer = ref('main')
const activeDM = ref(null)
const route = useRoute()

// Sample friends data
const friends = ref([
  {
    id: 1,
    username: 'Eris',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'online',
    activity: 'Playing Valorant'
  },
  {
    id: 2,
    username: 'Sylphie',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'away',
    activity: 'Away'
  },
  {
    id: 3,
    username: 'Roxy',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'busy',
    activity: 'In a meeting'
  }
])

// Computed
const showFriendsList = computed(() => {
  return route.name === 'DirectMessages' || route.path.includes('/dm/')
})

// Event handlers
const handleChannelSelect = (channelId) => {
  activeChannel.value = channelId
  activeDM.value = null
}

const handleDMSelect = (dmId) => {
  activeDM.value = dmId
  activeChannel.value = null
}

const handleServerSelect = (serverId) => {
  activeServer.value = serverId
}

const handleStartDM = (friendId) => {
  // Logic để bắt đầu DM với friend
  console.log('Starting DM with friend:', friendId)
}
</script>

<style scoped>
.app-container {
  display: flex;
  height: 100vh;
  background: #36393f;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
}

.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
  min-width: 0;
}
</style>
