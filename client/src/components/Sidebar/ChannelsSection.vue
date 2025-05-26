<template>
  <div class="section">
    <div class="section-header">
      <h3>{{ currentServer.name }}</h3>
      <button class="add-btn" @click="$emit('createNew')" title="Tạo kênh mới">
        <i class="bi bi-plus-lg"></i>
      </button>
    </div>
    
    <div class="categories-container">
      <!-- Nhóm Chat -->
      <div class="category-group">
        <div class="category-header" @click="$emit('toggleCategory', 'chat')">
          <i class="bi bi-chevron-right category-arrow" :class="{ 'rotate-90': chatExpanded }"></i>
          <i class="bi bi-chat-dots category-icon"></i>
          <span class="category-title">Nhóm Chat</span>
          <span class="category-count">({{ currentServer.textChannels.length }})</span>
        </div>
        
        <transition name="slide-down">
          <div v-show="chatExpanded" class="channels-list">
            <ChannelItem
              v-for="channel in currentServer.textChannels"
              :key="channel.id"
              :channel="channel"
              :isActive="activeChannel === channel.id"
              type="text"
              @selectChannel="$emit('selectChannel', channel.id)"
            />
          </div>
        </transition>
      </div>

      <!-- Nhóm Thoại -->
      <div class="category-group">
        <div class="category-header" @click="$emit('toggleCategory', 'voice')">
          <i class="bi bi-chevron-right category-arrow" :class="{ 'rotate-90': voiceExpanded }"></i>
          <i class="bi bi-volume-up category-icon"></i>
          <span class="category-title">Nhóm Thoại</span>
          <span class="category-count">({{ currentServer.voiceChannels.length }})</span>
        </div>
        
        <transition name="slide-down">
          <div v-show="voiceExpanded" class="channels-list">
            <ChannelItem
              v-for="channel in currentServer.voiceChannels"
              :key="channel.id"
              :channel="channel"
              :isActive="activeChannel === channel.id"
              type="voice"
              @selectChannel="$emit('selectChannel', channel.id)"
            />
            
            <!-- Voice channel users -->
            <div v-for="channel in currentServer.voiceChannels.filter(c => c.connectedUsers?.length > 0)" 
                 :key="`users-${channel.id}`" 
                 class="voice-users-list">
              <VoiceUserItem
                v-for="user in channel.connectedUsers" 
                :key="user.id" 
                :user="user"
              />
            </div>
          </div>
        </transition>
      </div>
    </div>
  </div>
</template>

<script setup>
import ChannelItem from './ChannelItem.vue'
import VoiceUserItem from './VoiceUserItem.vue'

defineProps({
  currentServer: {
    type: Object,
    required: true
  },
  activeChannel: {
    type: String,
    required: true
  },
  chatExpanded: {
    type: Boolean,
    default: true
  },
  voiceExpanded: {
    type: Boolean,
    default: true
  }
})

defineEmits(['selectChannel', 'toggleCategory', 'createNew'])
</script>

<style scoped>
.section {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.section-header {
  padding: 16px 16px 8px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-shrink: 0;
}

.section-header h3 {
  font-size: 12px;
  font-weight: 600;
  color: #b5bac1;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  margin: 0;
}

.add-btn {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 4px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 14px;
}

.add-btn:hover {
  background: #35373c;
  color: #f2f3f5;
}

.categories-container {
  flex: 1;
  overflow-y: auto;
  padding: 0 8px 16px;
}

.category-group {
  margin-bottom: 20px;
}

.category-header {
  display: flex;
  align-items: center;
  padding: 8px;
  cursor: pointer;
  transition: all 0.15s ease;
  margin-bottom: 6px;
  border-radius: 6px;
  background: #2b2d31;
}

.category-header:hover {
  background: #35373c;
}

.category-arrow {
  color: #b5bac1;
  transition: transform 0.15s ease;
  margin-right: 8px;
  font-size: 12px;
}

.category-arrow.rotate-90 {
  transform: rotate(90deg);
}

.category-icon {
  color: #b5bac1;
  margin-right: 8px;
  font-size: 14px;
}

.category-title {
  flex: 1;
  font-size: 13px;
  font-weight: 600;
  color: #f2f3f5;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.category-count {
  font-size: 11px;
  color: #80848e;
  margin-left: 4px;
}

.channels-list {
  margin-left: 16px;
}

.voice-users-list {
  margin-left: 32px;
  margin-bottom: 8px;
}

.slide-down-enter-active,
.slide-down-leave-active {
  transition: all 0.2s ease;
  overflow: hidden;
}

.slide-down-enter-from,
.slide-down-leave-to {
  max-height: 0;
  opacity: 0;
}

.slide-down-enter-to,
.slide-down-leave-from {
  max-height: 500px;
  opacity: 1;
}

.categories-container::-webkit-scrollbar {
  width: 4px;
}

.categories-container::-webkit-scrollbar-track {
  background: transparent;
}

.categories-container::-webkit-scrollbar-thumb {
  background: #2b2d31;
  border-radius: 2px;
}

.categories-container::-webkit-scrollbar-thumb:hover {
  background: #35373c;
}
</style>
