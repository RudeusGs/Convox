<template>
  <aside
    class="d-flex flex-column align-items-center bg-black py-3"
    style="width: 72px; min-height: 100vh;"
  >
    <ServerBarItem
      label="Convox"
      :avatar="null"
      :is-active="false"
      @select="$emit('select-home')"
    />

    <hr class="border-secondary my-3 w-75" />

    <!-- Các server / channel bạn tham gia -->
    <ServerBarItem
      v-for="server in servers"
      :key="server.id"
      :label="server.name"
      :avatar="server.avatar"
      :is-active="activeId === server.id"
      @select="onSelectServer(server)"
    />
    <div
      class="mt-auto d-flex flex-column align-items-center pt-3 gap-2 server-bar-bottom"
    >
      <ServerBarItem
        label="+"
        :avatar="null"
        :is-active="false"
        @select="onAddServer"
      />

      <ServerBarItem
        label="ø"
        :avatar="null"
        :is-active="false"
        @select="onExplore"
      />
    </div>
  </aside>
</template>

<script>
import ServerBarItem from './ServerBarItem.vue'

export default {
  name: 'ServerBar',
  components: { ServerBarItem },

  data() {
    return {
      activeId: 'sv1',
      // Tạm data cứng cho giống hình, sau bạn nối API / store vào
      servers: [
        { id: 'sv1', name: 'đội tôn lê', avatar: null },
        { id: 'sv2', name: 'máy của a', avatar: 'https://placekitten.com/80/80' },
        { id: 'sv3', name: '1', avatar: null },
        { id: 'sv4', name: 'B', avatar: null },
        { id: 'sv5', name: 'máy của ai', avatar: 'https://placehold.co/80x80' },
      ],
    }
  },

    methods: {
    onSelectServer(server) {
        this.activeId = server.id
        this.$emit('select-server', server)
    },
    onAddServer() {
        this.$emit('add-server')
    },
    onExplore() {
        this.$emit('explore')
    },
    },

}
</script>
<style scoped>
.server-bar-bottom {
  margin-bottom: 80px; /* chừa chỗ cho UserStatusCard, chỉnh số này theo chiều cao card */
}

@media (max-width: 768px) {
  .server-bar-bottom {
    margin-bottom: 60px; /* mobile card thấp hơn chút cho đỡ tốn chỗ */
  }
}
</style>
