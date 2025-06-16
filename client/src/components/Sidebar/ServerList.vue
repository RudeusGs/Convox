<template>
  <div class="fast-server-list">
    <!-- Main Server Bar -->
    <div class="server-bar">
      <!-- Current Server -->
      <div class="current-server" @click="toggleQuickPanel">
        <div class="server-icon current" :style="{ backgroundColor: currentServer?.color }">
          {{ currentServer?.name?.charAt(0) }}
        </div>
        <div v-if="currentServer?.unread" class="notification-dot"></div>
      </div>

      <!-- Divider -->
      <div class="server-divider"></div>

      <!-- Quick Access Servers -->
      <div class="quick-servers">
        <div 
          v-for="server in quickAccessServers" 
          :key="server.id"
          class="server-icon-wrapper"
          @click="selectServer(server.id)"
          @contextmenu.prevent="showContextMenu($event, server)"
          :title="server.name"
        >
          <div class="server-icon" :style="{ backgroundColor: server.color }">
            {{ server.name.charAt(0) }}
          </div>
          <div v-if="server.unread" class="notification-dot"></div>
          <div class="server-tooltip">{{ server.name }}</div>
        </div>
      </div>

      <!-- Add Server Button -->
      <button class="add-server-btn" @click="$emit('addServer')" title="Tham gia server">
        <i class="bi bi-plus"></i>
      </button>

      <!-- More Servers Button -->
      <button 
        class="more-servers-btn" 
        @click="toggleQuickPanel"
        :class="{ active: showQuickPanel }"
        title="Xem tất cả server"
      >
        <i class="bi bi-grid-3x3-gap"></i>
      </button>
    </div>

    <!-- Quick Access Panel -->
    <div v-if="showQuickPanel" class="quick-panel" @click.stop>
      <!-- Search -->
      <div class="panel-search">
        <div class="search-input">
          <i class="bi bi-search"></i>
          <input 
            v-model="searchQuery" 
            placeholder="Tìm server..."
            @input="handleSearch"
            ref="searchInput"
          />
        </div>
      </div>

      <!-- Server Sections -->
      <div class="panel-content">
        <!-- Favorites -->
        <div v-if="favoriteServers.length > 0" class="server-section">
          <h4 class="section-title">
            <i class="bi bi-star-fill"></i>
            Yêu thích
          </h4>
          <div class="server-list">
            <div 
              v-for="server in favoriteServers"
              :key="server.id"
              class="server-item"
              :class="{ active: activeServer === server.id }"
              @click="selectServer(server.id)"
              @contextmenu.prevent="showContextMenu($event, server)"
            >
              <div class="server-icon small" :style="{ backgroundColor: server.color }">
                {{ server.name.charAt(0) }}
              </div>
              <div class="server-info">
                <span class="server-name">{{ server.name }}</span>
                <span class="server-status">{{ server.memberCount }} thành viên</span>
              </div>
              <div class="server-actions">
                <div v-if="server.unread" class="unread-badge">{{ server.unread }}</div>
                <button class="action-btn" @click.stop="showContextMenu($event, server)">
                  <i class="bi bi-three-dots"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- All Servers -->
        <div class="server-section">
          <h4 class="section-title">
            <i class="bi bi-server"></i>
            {{ searchQuery ? 'Kết quả tìm kiếm' : 'Tất cả server' }}
            <span class="count">({{ filteredServers.length }})</span>
          </h4>
          <div class="server-list">
            <div 
              v-for="server in displayedServers"
              :key="server.id"
              class="server-item"
              :class="{ active: activeServer === server.id }"
              @click="selectServer(server.id)"
              @contextmenu.prevent="showContextMenu($event, server)"
            >
              <div class="server-icon small" :style="{ backgroundColor: server.color }">
                {{ server.name.charAt(0) }}
              </div>
              <div class="server-info">
                <span class="server-name">{{ server.name }}</span>
                <span class="server-status">
                  {{ server.memberCount }} thành viên • {{ formatLastAccess(server.lastAccessed) }}
                </span>
              </div>
              <div class="server-actions">
                <button 
                  class="action-btn favorite" 
                  :class="{ active: server.isFavorite }"
                  @click.stop="toggleFavorite(server.id)"
                  :title="server.isFavorite ? 'Bỏ yêu thích' : 'Thêm vào yêu thích'"
                >
                  <i :class="server.isFavorite ? 'bi-star-fill' : 'bi-star'"></i>
                </button>
                <div v-if="server.unread" class="unread-badge">{{ server.unread }}</div>
                <button class="action-btn" @click.stop="showContextMenu($event, server)">
                  <i class="bi bi-three-dots"></i>
                </button>
              </div>
            </div>
          </div>
          
          <!-- Load More -->
          <div v-if="hasMoreServers" class="load-more">
            <button class="load-more-btn" @click="loadMoreServers">
              Xem thêm {{ remainingCount }} server
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Context Menu -->
    <Teleport to="body">
      <div 
        v-if="contextMenu.show" 
        class="context-menu"
        :style="contextMenuStyle"
      >
        <div class="context-header">
          <div class="server-icon tiny" :style="{ backgroundColor: contextMenu.server?.color }">
            {{ contextMenu.server?.name?.charAt(0) }}
          </div>
          <span>{{ contextMenu.server?.name }}</span>
        </div>
        
        <div class="context-items">
          <button class="context-item" @click="inviteMembers">
            <i class="bi bi-person-plus"></i>
            Mời thành viên
          </button>
          
          <button class="context-item" @click="copyInvite">
            <i class="bi bi-link-45deg"></i>
            Sao chép link mời
          </button>
          
          <button class="context-item" @click="openSettings">
            <i class="bi bi-gear"></i>
            Cài đặt server
          </button>
          
          <div class="context-divider"></div>
          
          <button 
            class="context-item" 
            @click="toggleNotifications"
          >
            <i :class="contextMenu.server?.muted ? 'bi-bell' : 'bi-bell-slash'"></i>
            {{ contextMenu.server?.muted ? 'Bật thông báo' : 'Tắt thông báo' }}
          </button>
          
          <button 
            class="context-item" 
            :class="{ active: contextMenu.server?.isFavorite }"
            @click="toggleFavoriteFromMenu"
          >
            <i :class="contextMenu.server?.isFavorite ? 'bi-star-fill' : 'bi-star'"></i>
            {{ contextMenu.server?.isFavorite ? 'Bỏ yêu thích' : 'Thêm yêu thích' }}
          </button>
          
          <div class="context-divider"></div>
          
          <button class="context-item danger" @click="leaveServer">
            <i class="bi bi-box-arrow-right"></i>
            Rời server
          </button>
        </div>
      </div>
      
      <div v-if="contextMenu.show" class="context-overlay" @click="closeContextMenu"></div>
    </Teleport>

    <!-- Panel Overlay -->
    <div v-if="showQuickPanel" class="panel-overlay" @click="closeQuickPanel"></div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted, nextTick } from 'vue'

const props = defineProps({
  servers: {
    type: Array,
    required: true
  },
  activeServer: {
    type: String,
    required: true
  }
})

const emit = defineEmits(['selectServer', 'addServer', 'openServerSettings'])

// State
const showQuickPanel = ref(false)
const searchQuery = ref('')
const searchInput = ref(null)
const serversPerPage = ref(15)
const currentPage = ref(1)

const contextMenu = ref({
  show: false,
  server: null,
  x: 0,
  y: 0
})

// Enhanced servers data
const enhancedServers = ref(props.servers.map(server => ({
  ...server,
  isFavorite: ['main', 'gaming'].includes(server.id),
  memberCount: Math.floor(Math.random() * 500) + 50,
  lastAccessed: new Date(Date.now() - Math.random() * 7 * 24 * 60 * 60 * 1000),
  unread: Math.random() > 0.8 ? Math.floor(Math.random() * 5) + 1 : 0,
  muted: Math.random() > 0.7
})))

// Computed
const currentServer = computed(() => 
  enhancedServers.value.find(s => s.id === props.activeServer) || enhancedServers.value[0]
)

const favoriteServers = computed(() => 
  enhancedServers.value.filter(s => s.isFavorite)
)

const quickAccessServers = computed(() => {
  return enhancedServers.value
    .filter(s => s.id !== props.activeServer)
    .sort((a, b) => new Date(b.lastAccessed) - new Date(a.lastAccessed))
    .slice(0, 4)
})

const filteredServers = computed(() => {
  let servers = enhancedServers.value
  
  if (searchQuery.value) {
    servers = servers.filter(server =>
      server.name.toLowerCase().includes(searchQuery.value.toLowerCase())
    )
  }
  
  return servers.sort((a, b) => new Date(b.lastAccessed) - new Date(a.lastAccessed))
})

const displayedServers = computed(() => {
  const endIndex = currentPage.value * serversPerPage.value
  return filteredServers.value.slice(0, endIndex)
})

const hasMoreServers = computed(() => {
  return displayedServers.value.length < filteredServers.value.length
})

const remainingCount = computed(() => {
  return filteredServers.value.length - displayedServers.value.length
})

const contextMenuStyle = computed(() => {
  const menuWidth = 200
  const menuHeight = 280
  const padding = 10
  
  let x = contextMenu.value.x
  let y = contextMenu.value.y
  
  if (x + menuWidth + padding > window.innerWidth) {
    x = window.innerWidth - menuWidth - padding
  }
  
  if (y + menuHeight + padding > window.innerHeight) {
    y = window.innerHeight - menuHeight - padding
  }
  
  return {
    left: `${Math.max(padding, x)}px`,
    top: `${Math.max(padding, y)}px`
  }
})

// Methods
const toggleQuickPanel = () => {
  showQuickPanel.value = !showQuickPanel.value
  if (showQuickPanel.value) {
    searchQuery.value = ''
    currentPage.value = 1
    nextTick(() => {
      searchInput.value?.focus()
    })
  }
}

const closeQuickPanel = () => {
  showQuickPanel.value = false
}

const selectServer = (serverId) => {
  emit('selectServer', serverId)
  const server = enhancedServers.value.find(s => s.id === serverId)
  if (server) {
    server.lastAccessed = new Date()
  }
  closeQuickPanel()
}

const toggleFavorite = (serverId) => {
  const server = enhancedServers.value.find(s => s.id === serverId)
  if (server) {
    server.isFavorite = !server.isFavorite
  }
}

const loadMoreServers = () => {
  currentPage.value += 1
}

const handleSearch = () => {
  currentPage.value = 1
}

const formatLastAccess = (date) => {
  const now = new Date()
  const diff = now - date
  const hours = Math.floor(diff / (1000 * 60 * 60))
  const days = Math.floor(hours / 24)
  
  if (hours < 1) return 'Vừa xong'
  if (hours < 24) return `${hours}h trước`
  if (days === 1) return 'Hôm qua'
  if (days < 7) return `${days} ngày trước`
  return `${Math.floor(days / 7)} tuần trước`
}

const showContextMenu = (event, server) => {
  contextMenu.value = {
    show: true,
    server: server,
    x: event.clientX,
    y: event.clientY
  }
}

const closeContextMenu = () => {
  contextMenu.value.show = false
}

// Context menu actions
const inviteMembers = () => {
  console.log('Mời thành viên')
  closeContextMenu()
}

const copyInvite = () => {
  console.log('Sao chép link mời')
  closeContextMenu()
}

const openSettings = () => {
  emit('openServerSettings', contextMenu.value.server.id)
  closeContextMenu()
}

const toggleNotifications = () => {
  const server = enhancedServers.value.find(s => s.id === contextMenu.value.server.id)
  if (server) {
    server.muted = !server.muted
  }
  closeContextMenu()
}

const toggleFavoriteFromMenu = () => {
  toggleFavorite(contextMenu.value.server.id)
  closeContextMenu()
}

const leaveServer = () => {
  if (confirm(`Rời khỏi server "${contextMenu.value.server.name}"?`)) {
    const index = enhancedServers.value.findIndex(s => s.id === contextMenu.value.server.id)
    if (index > -1) {
      enhancedServers.value.splice(index, 1)
    }
  }
  closeContextMenu()
}

// Event handlers
const handleKeydown = (event) => {
  if (event.key === 'Escape') {
    if (contextMenu.value.show) {
      closeContextMenu()
    } else if (showQuickPanel.value) {
      closeQuickPanel()
    }
  }
}

const handleClickOutside = (event) => {
  if (contextMenu.value.show && !event.target.closest('.context-menu')) {
    closeContextMenu()
  }
}

onMounted(() => {
  document.addEventListener('keydown', handleKeydown)
  document.addEventListener('click', handleClickOutside)
})

onUnmounted(() => {
  document.removeEventListener('keydown', handleKeydown)
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.fast-server-list {
  position: relative;
}

/* Server Bar */
.server-bar {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 12px 8px;
  background: #1e1f22;
  border-right: 1px solid #2b2d31;
  width: 72px;
  height: 100vh;
  position: fixed;
  left: 0;
  top: 0;
  z-index: 100;
  gap: 8px;
  overflow-y: auto;
}

.current-server {
  position: relative;
  cursor: pointer;
  margin-bottom: 8px;
}

.server-icon {
  width: 48px;
  height: 48px;
  border-radius: 16px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 18px;
  transition: all 0.15s ease;
  position: relative;
}

.server-icon.current {
  border-radius: 16px;
}

.server-icon.small {
  width: 32px;
  height: 32px;
  font-size: 14px;
  border-radius: 8px;
}

.server-icon.tiny {
  width: 20px;
  height: 20px;
  font-size: 10px;
  border-radius: 6px;
}

.server-icon-wrapper {
  position: relative;
  cursor: pointer;
}

.server-icon-wrapper:hover .server-icon {
  border-radius: 12px;
}

.server-icon-wrapper:hover .server-tooltip {
  opacity: 1;
  visibility: visible;
  transform: translateX(0);
}

.server-tooltip {
  position: absolute;
  left: 60px;
  top: 50%;
  transform: translateY(-50%) translateX(-10px);
  background: #18191c;
  color: #ffffff;
  padding: 8px 12px;
  border-radius: 6px;
  font-size: 14px;
  font-weight: 500;
  white-space: nowrap;
  opacity: 0;
  visibility: hidden;
  transition: all 0.2s ease;
  z-index: 1000;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);
}

.server-tooltip::before {
  content: '';
  position: absolute;
  left: -4px;
  top: 50%;
  transform: translateY(-50%);
  border: 4px solid transparent;
  border-right-color: #18191c;
}

.notification-dot {
  position: absolute;
  top: -2px;
  right: -2px;
  width: 12px;
  height: 12px;
  background: #ed4245;
  border-radius: 50%;
  border: 2px solid #1e1f22;
}

.server-divider {
  width: 32px;
  height: 2px;
  background: #35373c;
  border-radius: 1px;
  margin: 4px 0;
}

.quick-servers {
  display: flex;
  flex-direction: column;
  gap: 8px;
  flex: 1;
}

.add-server-btn,
.more-servers-btn {
  width: 48px;
  height: 48px;
  border-radius: 16px;
  background: #35373c;
  border: none;
  color: #23a559;
  cursor: pointer;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
}

.add-server-btn:hover {
  border-radius: 12px;
  background: #23a559;
  color: white;
}

.more-servers-btn {
  color: #b5bac1;
  margin-top: 8px;
}

.more-servers-btn:hover,
.more-servers-btn.active {
  border-radius: 12px;
  background: #5865f2;
  color: white;
}

/* Quick Panel */
.panel-overlay {
  position: fixed;
  top: 0;
  left: 72px;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.3);
  z-index: 999;
}

.quick-panel {
  position: fixed;
  left: 72px;
  top: 0;
  width: 320px;
  height: 100vh;
  background: #2f3136;
  border-right: 1px solid #202225;
  z-index: 1000;
  display: flex;
  flex-direction: column;
  animation: slideIn 0.2s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateX(-100%);
  }
  to {
    transform: translateX(0);
  }
}

.panel-search {
  padding: 16px;
  border-bottom: 1px solid #40444b;
  background: #36393f;
}

.search-input {
  position: relative;
  display: flex;
  align-items: center;
}

.search-input i {
  position: absolute;
  left: 12px;
  color: #72767d;
  font-size: 14px;
  z-index: 1;
}

.search-input input {
  width: 100%;
  background: #40444b;
  border: none;
  border-radius: 6px;
  padding: 8px 12px 8px 36px;
  color: #ffffff;
  font-size: 14px;
  outline: none;
  transition: all 0.15s ease;
}

.search-input input:focus {
  background: #484c52;
}

.search-input input::placeholder {
  color: #72767d;
}

.panel-content {
  flex: 1;
  overflow-y: auto;
  padding: 8px 0;
}

.server-section {
  margin-bottom: 16px;
}

.section-title {
  font-size: 12px;
  font-weight: 600;
  color: #b5bac1;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0 0 8px 0;
  padding: 8px 16px 0;
  display: flex;
  align-items: center;
  gap: 6px;
}

.count {
  color: #72767d;
  font-weight: 400;
}

.server-list {
  display: flex;
  flex-direction: column;
}

.server-item {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  cursor: pointer;
  transition: all 0.15s ease;
  position: relative;
}

.server-item:hover {
  background: #35373c;
}

.server-item.active {
  background: rgba(88, 101, 242, 0.1);
  border-left: 3px solid #5865f2;
}

.server-info {
  flex: 1;
  margin-left: 12px;
  min-width: 0;
}

.server-name {
  font-size: 14px;
  font-weight: 500;
  color: #ffffff;
  display: block;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  line-height: 1.2;
}

.server-status {
  font-size: 12px;
  color: #b5bac1;
  line-height: 1.2;
}

.server-actions {
  display: flex;
  align-items: center;
  gap: 4px;
  opacity: 0;
  transition: opacity 0.15s ease;
}

.server-item:hover .server-actions {
  opacity: 1;
}

.action-btn {
  background: none;
  border: none;
  color: #72767d;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 12px;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.action-btn:hover {
  background: #40444b;
  color: #ffffff;
}

.action-btn.favorite.active {
  color: #faa61a;
}

.unread-badge {
  background: #ed4245;
  color: white;
  font-size: 10px;
  font-weight: 600;
  padding: 2px 6px;
  border-radius: 10px;
  min-width: 16px;
  text-align: center;
  line-height: 1.2;
}

.load-more {
  padding: 8px 16px;
}

.load-more-btn {
  width: 100%;
  background: #40444b;
  border: none;
  color: #b5bac1;
  padding: 8px 12px;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.15s ease;
  font-size: 13px;
}

.load-more-btn:hover {
  background: #484c52;
  color: #ffffff;
}

/* Context Menu */
.context-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: transparent;
  z-index: 1999;
}

.context-menu {
  position: fixed;
  background: #18191c;
  border: 1px solid #2f3136;
  border-radius: 6px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3);
  z-index: 2000;
  min-width: 200px;
  overflow: hidden;
  animation: contextFadeIn 0.1s ease-out;
}

@keyframes contextFadeIn {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.context-header {
  padding: 8px 12px;
  border-bottom: 1px solid #2f3136;
  display: flex;
  align-items: center;
  gap: 8px;
  background: #2f3136;
  font-size: 13px;
  font-weight: 500;
  color: #ffffff;
}

.context-items {
  padding: 4px 0;
}

.context-item {
  width: 100%;
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 8px 12px;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  gap: 10px;
  font-size: 13px;
  text-align: left;
}

.context-item:hover {
  background: #4752c4;
  color: #ffffff;
}

.context-item.active {
  color: #faa61a;
}

.context-item.danger {
  color: #ed4245;
}

.context-item.danger:hover {
  background: #ed4245;
  color: #ffffff;
}

.context-divider {
  height: 1px;
  background: #2f3136;
  margin: 4px 0;
}

/* Scrollbar */
.server-bar::-webkit-scrollbar,
.panel-content::-webkit-scrollbar {
  width: 4px;
}

.server-bar::-webkit-scrollbar-track,
.panel-content::-webkit-scrollbar-track {
  background: transparent;
}

.server-bar::-webkit-scrollbar-thumb,
.panel-content::-webkit-scrollbar-thumb {
  background: #202225;
  border-radius: 2px;
}

.server-bar::-webkit-scrollbar-thumb:hover,
.panel-content::-webkit-scrollbar-thumb:hover {
  background: #2b2d31;
}

/* Mobile Responsive */
@media (max-width: 768px) {
  .server-bar {
    width: 60px;
    padding: 8px 6px;
  }
  
  .server-icon {
    width: 40px;
    height: 40px;
    font-size: 16px;
  }
  
  .add-server-btn,
  .more-servers-btn {
    width: 40px;
    height: 40px;
    font-size: 16px;
  }
  
  .quick-panel {
    left: 60px;
    width: calc(100vw - 60px);
  }
  
  .server-tooltip {
    display: none;
  }
}
</style>