<template>
  <div class="modern-sidebar">
    <SidebarHeader />
    <ServerList 
      :servers="servers"
      :activeServer="activeServer"
      @selectServer="selectServer"
      @addServer="addServer"
    />
    <NavigationTabs 
      :tabs="tabs"
      :activeTab="activeTab"
      @setActiveTab="setActiveTab"
    />
    
    <div class="content-area">
      <ChannelsSection 
        v-if="activeTab === 'channels'"
        :currentServer="currentServer"
        :activeChannel="activeChannel"
        :chatExpanded="chatExpanded"
        :voiceExpanded="voiceExpanded"
        @selectChannel="selectChannel"
        @toggleCategory="toggleCategory"
        @createNew="createNew"
      />
      
      <DirectMessagesSection 
        v-if="activeTab === 'messages'"
        :directMessages="directMessages"
        :activeDM="activeDM"
        @selectDM="selectDM"
        @startNewDM="startNewDM"
      />
      
      <MembersSection 
        v-if="activeTab === 'members'"
        :members="members"
        :onlineMembers="onlineMembers"
        :totalMembers="totalMembers"
      />
    </div>

    <UserProfile 
      :user="user"
      :userStatus="userStatus"
      :isMuted="isMuted"
      :isDeafened="isDeafened"
      @toggleMute="toggleMute"
      @toggleDeafen="toggleDeafen"
      @openSettings="openSettings"
      @toggleProfileMenu="toggleProfileMenu"
    />
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'
import SidebarHeader from './SidebarHeader.vue'
import ServerList from './ServerList.vue'
import NavigationTabs from './NavigationTabs.vue'
import ChannelsSection from './ChannelsSection.vue'
import DirectMessagesSection from './DirectMessagesSection.vue'
import MembersSection from './MembersSection.vue'
import UserProfile from './UserProfile.vue'

// State
const activeTab = ref('channels')
const activeServer = ref('main')
const activeChannel = ref('general')
const activeDM = ref(null)
const isMuted = ref(false)
const isDeafened = ref(false)
const userStatus = ref('online')
const onlineMembers = ref(47)
const totalMembers = ref(234)
const chatExpanded = ref(true)
const voiceExpanded = ref(true)

// Data
const user = ref({
  name: 'Rudeus Greyrat',
  avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s'
})

const servers = ref([
  { id: 'main', name: 'Convox Team', color: '#5865f2', unread: true },
  { id: 'gaming', name: 'Gaming Hub', color: '#57f287', unread: false },
  { id: 'study', name: 'Study Group', color: '#fee75c', unread: true },
  { id: 'work', name: 'Work Space', color: '#eb459e', unread: false },
  { id: 'friends', name: 'Friends', color: '#ff7a6b', unread: false }
])

const tabs = ref([
  { id: 'channels', label: 'Kênh', icon: 'bi-hash', count: null },
  { id: 'messages', label: 'Tin nhắn', icon: 'bi-chat-dots', count: 3 },
  { id: 'members', label: 'Thành viên', icon: 'bi-people', count: null }
])

const serversData = ref({
  main: {
    name: 'Convox Team',
    textChannels: [
      { id: 'rules', name: 'quy-tắc', unread: false, locked: true },
      { id: 'announcements', name: 'thông-báo', unread: true, unreadCount: 2, locked: true },
      { id: 'general', name: 'chung', unread: true, unreadCount: 12 },
      { id: 'random', name: 'ngẫu-nhiên', unread: false },
      { id: 'memes', name: 'memes', unread: true, unreadCount: 5 },
      { id: 'frontend', name: 'frontend', unread: true, unreadCount: 3 },
      { id: 'backend', name: 'backend', unread: false },
      { id: 'design', name: 'thiết-kế', unread: true, unreadCount: 1 },
      { id: 'testing', name: 'testing', unread: false }
    ],
    voiceChannels: [
      { 
        id: 'lobby', 
        name: 'Sảnh chờ', 
        activeUsers: 3,
        connectedUsers: [
          { id: 'u1', name: 'Alice', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false },
          { id: 'u2', name: 'Bob', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: true, deafened: false },
          { id: 'u3', name: 'Carol', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: true }
        ]
      },
      { 
        id: 'meeting', 
        name: 'Họp nhóm', 
        activeUsers: 5,
        connectedUsers: [
          { id: 'u4', name: 'David', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false },
          { id: 'u5', name: 'Eve', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false },
          { id: 'u6', name: 'Frank', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: true, deafened: false },
          { id: 'u7', name: 'Grace', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false },
          { id: 'u8', name: 'Henry', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: true }
        ]
      },
      { id: 'casual', name: 'Trò chuyện', activeUsers: 0 },
      { 
        id: 'code-review', 
        name: 'Code Review', 
        activeUsers: 2,
        connectedUsers: [
          { id: 'u9', name: 'Ian', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false },
          { id: 'u10', name: 'Jane', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', muted: false, deafened: false }
        ]
      },
      { id: 'brainstorm', name: 'Brainstorm', activeUsers: 0 }
    ]
  },
  gaming: {
    name: 'Gaming Hub',
    textChannels: [
      { id: 'general-gaming', name: 'chung', unread: false },
      { id: 'lfg', name: 'tìm-nhóm', unread: true, unreadCount: 7 },
      { id: 'valorant', name: 'valorant', unread: true, unreadCount: 15 },
      { id: 'lol', name: 'league-of-legends', unread: false },
      { id: 'minecraft', name: 'minecraft', unread: true, unreadCount: 3 }
    ],
    voiceChannels: [
      { id: 'game-room-1', name: 'Game Room 1', activeUsers: 4 },
      { id: 'game-room-2', name: 'Game Room 2', activeUsers: 0 },
      { id: 'chill-gaming', name: 'Chill Gaming', activeUsers: 2 }
    ]
  },
  study: {
    name: 'Study Group',
    textChannels: [
      { id: 'general-study', name: 'chung', unread: true, unreadCount: 3 },
      { id: 'homework', name: 'bài-tập', unread: false },
      { id: 'resources', name: 'tài-liệu', unread: true, unreadCount: 1 }
    ],
    voiceChannels: [
      { id: 'study-room', name: 'Phòng học', activeUsers: 0 },
      { id: 'group-study', name: 'Học nhóm', activeUsers: 3 }
    ]
  }
})

const currentServer = computed(() => serversData.value[activeServer.value] || serversData.value.main)

const directMessages = ref([
  {
    id: 'dm1',
    name: 'Alice Johnson',
    avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s',
    status: 'online',
    lastMessage: 'Có thể gặp lúc 3h được không?',
    time: '2p',
    unread: true,
    unreadCount: 2
  },
  {
    id: 'dm2',
    name: 'Bob Smith',
    avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s',
    status: 'away',
    lastMessage: 'Cảm ơn bạn!',
    time: '1h',
    unread: false
  },
  {
    id: 'dm3',
    name: 'Carol Davis',
    avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s',
    status: 'offline',
    lastMessage: 'Hẹn gặp lại sau',
    time: '1d',
    unread: false
  },
  {
    id: 'dm4',
    name: 'David Wilson',
    avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s',
    status: 'busy',
    lastMessage: 'Đang bận, chat sau nhé',
    time: '2h',
    unread: true,
    unreadCount: 1
  }
])

const members = ref([
  { id: 'member1', name: 'Alice Johnson', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'online', role: 'Admin' },
  { id: 'member2', name: 'Bob Smith', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'away', role: 'Moderator' },
  { id: 'member3', name: 'Carol Davis', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'online', role: 'Member' },
  { id: 'member4', name: 'David Wilson', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'busy', role: 'Developer' },
  { id: 'member5', name: 'Eve Brown', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'online', role: 'Designer' },
  { id: 'member6', name: 'Frank Miller', avatar: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcTYIINC3-ky-iCYztrC569il7bigltnIx8O1g&s', status: 'offline', role: 'Member' }
])

// Methods
const addServer = () => {
  console.log('Thêm server mới')
}

const selectServer = (serverId) => {
  activeServer.value = serverId
  if (serversData.value[serverId]?.textChannels.length > 0) {
    activeChannel.value = serversData.value[serverId].textChannels[0].id
  }
}

const createNew = () => {
  console.log('Tạo mới')
}

const setActiveTab = (tabId) => {
  activeTab.value = tabId
}

const toggleCategory = (categoryType) => {
  if (categoryType === 'chat') {
    chatExpanded.value = !chatExpanded.value
  } else if (categoryType === 'voice') {
    voiceExpanded.value = !voiceExpanded.value
  }
}

const selectChannel = (channelId) => {
  activeChannel.value = channelId
  activeDM.value = null
}

const selectDM = (dmId) => {
  activeDM.value = dmId
  activeChannel.value = null
}

const startNewDM = () => {
  console.log('Bắt đầu tin nhắn mới')
}

const toggleProfileMenu = () => {
  console.log('Toggle profile menu')
}

const toggleMute = () => {
  isMuted.value = !isMuted.value
}

const toggleDeafen = () => {
  isDeafened.value = !isDeafened.value
}

const openSettings = () => {
  console.log('Mở cài đặt')
}
</script>

<style scoped>
.modern-sidebar {
  width: 280px;
  height: 100vh;
  background: linear-gradient(180deg, #1e1f22 0%, #1a1b1e 100%);
  border-right: 1px solid #2b2d31;
  display: flex;
  flex-direction: column;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
  overflow: hidden;
}

.content-area {
  flex: 1;
  overflow: hidden;
  display: flex;
  flex-direction: column;
}
</style>
