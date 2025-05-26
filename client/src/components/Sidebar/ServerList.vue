<template>
  <div class="server-list">
    <div 
      v-for="server in servers" 
      :key="server.id"
      class="server-item"
      :class="{ active: activeServer === server.id }"
      @click="$emit('selectServer', server.id)"
      :title="server.name"
    >
      <div class="server-icon" :style="{ backgroundColor: server.color }">
        {{ server.name.charAt(0) }}
      </div>
      <div v-if="server.unread" class="server-notification"></div>
    </div>
    <button class="add-server-btn" @click="$emit('addServer')" title="Thêm server">
      <i class="bi bi-plus"></i>
    </button>
  </div>
</template>

<script setup>
defineProps({
  servers: {
    type: Array,
    required: true
  },
  activeServer: {
    type: String,
    required: true
  }
})

defineEmits(['selectServer', 'addServer'])
</script>

<style scoped>
.server-list {
  padding: 12px 8px;
  border-bottom: 1px solid #2b2d31;
  display: flex;
  gap: 8px;
  overflow-x: auto;
  flex-shrink: 0;
}

.server-item {
  position: relative;
  width: 40px;
  height: 40px;
  border-radius: 12px;
  cursor: pointer;
  transition: all 0.15s ease;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
}

.server-item:hover {
  border-radius: 16px;
}

.server-item.active {
  border-radius: 16px;
}

.server-icon {
  width: 100%;
  height: 100%;
  border-radius: inherit;
  display: flex;
  align-items: center;
  justify-content: center;
  color: white;
  font-weight: 600;
  font-size: 14px;
}

.server-notification {
  position: absolute;
  top: -2px;
  right: -2px;
  width: 8px;
  height: 8px;
  background: #ed4245;
  border-radius: 50%;
  border: 2px solid #1e1f22;
}

.add-server-btn {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  background: #35373c;
  border: none;
  color: #23a55a;
  cursor: pointer;
  transition: all 0.15s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 16px;
  flex-shrink: 0;
}

.add-server-btn:hover {
  border-radius: 16px;
  background: #23a55a;
  color: white;
}

.server-list::-webkit-scrollbar {
  height: 4px;
}

.server-list::-webkit-scrollbar-track {
  background: transparent;
}

.server-list::-webkit-scrollbar-thumb {
  background: #2b2d31;
  border-radius: 2px;
}

.server-list::-webkit-scrollbar-thumb:hover {
  background: #35373c;
}
</style>
