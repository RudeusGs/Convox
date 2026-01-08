<template>
  <div class="d-flex">
    <ServerBar
      @select-server="onSelectServer"
      @add-server="showCreateModal = true"
    />

    <div class="flex-grow-1 d-flex">
      <RouterView
        :activeChannel="activeChannel"
        :activeServer="activeServer"
        :activeDM="activeDM"
      />
    </div>

    <!-- Modal tạo phòng -->
    <CreateServerModal
      v-if="showCreateModal"
      @close="showCreateModal = false"
      @create="handleCreateServer"
      @open-join-by-code="openJoinByCodeModal"
    />

    <!-- Modal join bằng mã -->
    <ModalJoinRoom
      v-if="showJoinModal"
      @close="showJoinModal = false"
      @join="handleJoinByCode"
    />

    <!-- Card user cố định góc trái dưới -->
    <UserStatusCard
      v-if="isLoggedIn"
      :displayName="currentUser.displayName"
      :avatarUrl="currentUser.avatar"
      :statusText="currentUser.statusText"
      :statusType="currentUser.statusType"
      @toggle-mic="onToggleMic"
      @toggle-audio="onToggleAudio"
      @toggle-camera="onToggleCamera"
      @open-settings="onOpenSettings"
    />
  </div>
</template>

<script>
import ServerBar from '@/components/ServerBar/ServerBar.vue'
import CreateServerModal from '@/components/modals/CreateServerModal.vue'
import ModalJoinRoom from '@/components/modals/ModalJoinRoom.vue'
import UserStatusCard from '@/components/UserStatusCard.vue'

export default {
  name: 'MainLayout',
  components: {
    ServerBar,
    CreateServerModal,
    ModalJoinRoom,
    UserStatusCard,
  },

  data() {
    return {
      activeServer: null,
      activeChannel: null,
      activeDM: null,
      showCreateModal: false,
      showJoinModal: false,        // <<< thêm state modal join
      isLoggedIn: true,
      currentUser: {
        displayName: 'Rudeus Grey',
        avatar: 'https://placehold.co/80x80?text=RG',
        statusText: 'Trực tuyến',
        statusType: 'online',
      },
    }
  },

  methods: {
    onSelectServer(server) {
      this.activeServer = server
      console.log('Selected server:', server)
    },

    handleCreateServer(payload) {
      console.log('Tạo server mới:', payload)
      this.showCreateModal = false
    },

    // từ ModalJoinRoom @join
    handleJoinByCode(payload) {
      // payload có thể là string hoặc object { code, nickname }
      const code = typeof payload === 'string' ? payload : payload.code
      const nickname = typeof payload === 'string' ? null : payload.nickname

      console.log('Tham gia server bằng mã:', code, 'nickname:', nickname)
      // TODO: call API join server ở đây
      this.showJoinModal = false
    },

    // từ CreateServerModal @open-join-by-code
    openJoinByCodeModal() {
      this.showCreateModal = false
      this.showJoinModal = true
    },

    onToggleMic() {
      console.log('toggle mic')
    },
    onToggleAudio() {
      console.log('toggle audio')
    },
    onToggleCamera() {
      console.log('toggle camera')
    },
    onOpenSettings() {
      console.log('open settings')
    },
  },
}
</script>
