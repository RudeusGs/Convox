<template>
  <div class="sidebar-header">
    <div class="logo-section">
      <div class="logo-icon">
        <div class="logo-gradient"></div>
        <span class="logo-text">C</span>
      </div>
      <div class="workspace-info">
        <h2 class="workspace-name">Convox</h2>
        <span class="workspace-status">{{ onlineMembers }} online</span>
      </div>
    </div>
    <button class="menu-toggle" @click="toggleSidebar">
      <i class="bi bi-three-dots"></i>
    </button>
  </div>
</template>

<script setup>
defineProps({
  onlineMembers: {
    type: Number,
    default: 47
  }
})

const toggleSidebar = () => {
  console.log('Toggle sidebar')
}
</script>

<style scoped>
.sidebar-header {
  padding: 16px;
  border-bottom: 1px solid #2b2d31;
  display: flex;
  align-items: center;
  justify-content: space-between;
  flex-shrink: 0;
}

.logo-section {
  display: flex;
  align-items: center;
  gap: 12px;
  min-width: 0;
  flex: 1;
}

.logo-icon {
  position: relative;
  width: 36px;
  height: 36px;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  flex-shrink: 0;
}

.logo-gradient {
  position: absolute;
  inset: 0;
  background: linear-gradient(135deg, #5865f2 0%, #7289da 100%);
}

.logo-text {
  position: relative;
  color: white;
  font-weight: 700;
  font-size: 16px;
}

.workspace-info {
  display: flex;
  flex-direction: column;
  min-width: 0;
  flex: 1;
}

.workspace-name {
  font-size: 15px;
  font-weight: 600;
  color: #f2f3f5;
  margin: 0;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.workspace-status {
  font-size: 11px;
  color: #b5bac1;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.menu-toggle {
  background: none;
  border: none;
  color: #b5bac1;
  cursor: pointer;
  padding: 6px;
  border-radius: 4px;
  transition: all 0.15s ease;
  font-size: 16px;
  flex-shrink: 0;
}

.menu-toggle:hover {
  background: #35373c;
  color: #f2f3f5;
}
</style>
