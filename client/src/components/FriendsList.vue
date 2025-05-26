<template>
  <div class="friends-list">
    <!-- Friends Header -->
    <div class="friends-header">
      <h3 class="friends-title">Online — {{ onlineFriends.length }}</h3>
      <button class="friends-action" title="Start Group DM">
        <i class="bi bi-chat-dots-fill"></i>
      </button>
    </div>

    <!-- Friends Container -->
    <div class="friends-container">
      <!-- Online Friends -->
      <div v-for="friend in onlineFriends" :key="friend.id" class="friend-item">
        <div class="friend-avatar">
          <img :src="friend.avatar" :alt="friend.username" />
          <div class="friend-status" :class="friend.status"></div>
        </div>
        <div class="friend-info">
          <div class="friend-username">{{ friend.username }}</div>
          <div class="friend-activity">{{ friend.activity }}</div>
        </div>
        <div class="friend-actions">
          <button class="friend-action" title="Send Message">
            <i class="bi bi-chat-fill"></i>
          </button>
          <button class="friend-action" title="Start Voice Call">
            <i class="bi bi-telephone-fill"></i>
          </button>
          <button class="friend-action" title="Start Video Call">
            <i class="bi bi-camera-video-fill"></i>
          </button>
          <button class="friend-action" title="More">
            <i class="bi bi-three-dots-vertical"></i>
          </button>
        </div>
      </div>

      <!-- Offline Section -->
      <div class="offline-section">
        <div class="offline-header" @click="toggleOffline">
          <i class="bi bi-chevron-right offline-arrow" :class="{ expanded: showOffline }"></i>
          <span class="offline-title">Offline — {{ offlineFriends.length }}</span>
        </div>
        
        <div v-show="showOffline" class="offline-friends">
          <div v-for="friend in offlineFriends" :key="friend.id" class="friend-item offline">
            <div class="friend-avatar">
              <img :src="friend.avatar" :alt="friend.username" />
              <div class="friend-status offline"></div>
            </div>
            <div class="friend-info">
              <div class="friend-username">{{ friend.username }}</div>
              <div class="friend-activity">Offline</div>
            </div>
            <div class="friend-actions">
              <button class="friend-action" title="Send Message">
                <i class="bi bi-chat-fill"></i>
              </button>
              <button class="friend-action" title="More">
                <i class="bi bi-three-dots-vertical"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Now Playing -->
    <div class="now-playing">
      <div class="now-playing-header">
        <h4>Activity</h4>
      </div>
      <div class="activity-item">
        <div class="activity-icon">
          <i class="bi bi-controller"></i>
        </div>
        <div class="activity-info">
          <div class="activity-name">Playing Cyberpunk 2077</div>
          <div class="activity-details">2 hours elapsed</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue'

// State
const showOffline = ref(false)

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
  },
  {
    id: 4,
    username: 'Paul',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'online',
    activity: 'Listening to Spotify'
  },
  {
    id: 5,
    username: 'Zenith',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'online',
    activity: 'Watching YouTube'
  },
  {
    id: 6,
    username: 'Ghislaine',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'offline',
    activity: 'Last seen 2 hours ago'
  },
  {
    id: 7,
    username: 'Ruijerd',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'offline',
    activity: 'Last seen yesterday'
  },
  {
    id: 8,
    username: 'Orsted',
    avatar: '/placeholder.svg?height=32&width=32',
    status: 'offline',
    activity: 'Last seen 3 days ago'
  }
])

// Computed
const onlineFriends = computed(() => {
  return friends.value.filter(friend => friend.status !== 'offline')
})

const offlineFriends = computed(() => {
  return friends.value.filter(friend => friend.status === 'offline')
})

// Methods
const toggleOffline = () => {
  showOffline.value = !showOffline.value
}
</script>

<style scoped>
.friends-list {
  width: 240px;
  background: #2f3136;
  display: flex;
  flex-direction: column;
  border-left: 1px solid #202225;
}

/* Friends Header */
.friends-header {
  padding: 16px;
  border-bottom: 1px solid #202225;
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.friends-title {
  font-size: 12px;
  font-weight: 600;
  color: #8e9297;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  margin: 0;
}

.friends-action {
  background: none;
  border: none;
  color: #8e9297;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease-out;
  font-size: 16px;
}

.friends-action:hover {
  color: #dcddde;
  background: #40444b;
}

/* Friends Container */
.friends-container {
  flex: 1;
  overflow-y: auto;
  padding: 8px 0;
}

.friend-item {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  cursor: pointer;
  transition: background 0.15s ease-out;
  position: relative;
}

.friend-item:hover {
  background: #34373c;
}

.friend-item:hover .friend-actions {
  opacity: 1;
}

.friend-item.offline {
  opacity: 0.6;
}

.friend-avatar {
  position: relative;
  margin-right: 12px;
  flex-shrink: 0;
}

.friend-avatar img {
  width: 32px;
  height: 32px;
  border-radius: 50%;
}

.friend-status {
  position: absolute;
  bottom: -2px;
  right: -2px;
  width: 12px;
  height: 12px;
  border-radius: 50%;
  border: 3px solid #2f3136;
}

.friend-status.online {
  background: #43b581;
}

.friend-status.away {
  background: #faa61a;
}

.friend-status.busy {
  background: #f04747;
}

.friend-status.offline {
  background: #747f8d;
}

.friend-info {
  flex: 1;
  min-width: 0;
}

.friend-username {
  font-size: 14px;
  font-weight: 500;
  color: #ffffff;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.friend-activity {
  font-size: 12px;
  color: #b9bbbe;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.friend-actions {
  display: flex;
  gap: 4px;
  opacity: 0;
  transition: opacity 0.15s ease-out;
}

.friend-action {
  background: none;
  border: none;
  color: #b9bbbe;
  cursor: pointer;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.15s ease-out;
  font-size: 14px;
}

.friend-action:hover {
  background: #40444b;
  color: #dcddde;
}

/* Offline Section */
.offline-section {
  margin-top: 16px;
}

.offline-header {
  display: flex;
  align-items: center;
  padding: 8px 16px;
  cursor: pointer;
  transition: color 0.15s ease-out;
}

.offline-header:hover {
  color: #dcddde;
}

.offline-arrow {
  margin-right: 4px;
  font-size: 10px;
  color: #8e9297;
  transition: transform 0.15s ease-out;
}

.offline-arrow.expanded {
  transform: rotate(90deg);
}

.offline-title {
  font-size: 12px;
  font-weight: 600;
  color: #8e9297;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.offline-friends {
  margin-top: 4px;
}

/* Now Playing */
.now-playing {
  padding: 16px;
  border-top: 1px solid #202225;
  background: #292b2f;
}

.now-playing-header {
  margin-bottom: 12px;
}

.now-playing-header h4 {
  font-size: 12px;
  font-weight: 600;
  color: #8e9297;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  margin: 0;
}

.activity-item {
  display: flex;
  align-items: center;
  gap: 12px;
}

.activity-icon {
  width: 32px;
  height: 32px;
  background: #5865f2;
  border-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #ffffff;
  font-size: 16px;
}

.activity-info {
  flex: 1;
  min-width: 0;
}

.activity-name {
  font-size: 14px;
  font-weight: 500;
  color: #ffffff;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.activity-details {
  font-size: 12px;
  color: #b9bbbe;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* Scrollbar */
.friends-container::-webkit-scrollbar {
  width: 8px;
}

.friends-container::-webkit-scrollbar-track {
  background: transparent;
}

.friends-container::-webkit-scrollbar-thumb {
  background: #202225;
  border-radius: 4px;
}

.friends-container::-webkit-scrollbar-thumb:hover {
  background: #1a1c1f;
}
</style>