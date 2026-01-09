<template>
  <div class="d-flex">
    <ServerBar
      @select-server="onSelectServer"
      @add-server="showCreateModal = true"
    />

    <div class="flex-grow-1 d-flex">
      <ServerSidebar
        v-if="activeServer"
        :server="activeServer"
        :activeChannelId="activeChannel && activeChannel.id"
        @select-channel="onSelectChannel"
        @join-voice="onJoinVoice"
        @close-sidebar="onCloseSidebar"
      />

      <div class="flex-grow-1 d-flex">
        <RouterView
          :activeChannel="activeChannel"
          :activeServer="activeServer"
          :activeDM="activeDM"
        />
      </div>
    </div>

    <CreateServerModal
      v-if="showCreateModal"
      @close="showCreateModal = false"
      @create="handleCreateServer"
      @open-join-by-code="openJoinByCodeModal"
    />

    <ModalJoinRoom
      v-if="showJoinModal"
      @close="showJoinModal = false"
      @join="handleJoinByCode"
    />

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
import ServerSidebar from '@/components/Sidebar/ServerSidebar.vue'
import CreateServerModal from '@/components/modals/CreateServerModal.vue'
import ModalJoinRoom from '@/components/modals/ModalJoinRoom.vue'
import UserStatusCard from '@/components/UserStatusCard.vue'

export default {
  name: 'MainLayout',
  components: {
    ServerBar,
    ServerSidebar,
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
      showJoinModal: false,
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
      const firstSection =
        server.sections && server.sections.length
          ? server.sections[0]
          : null
      const firstChannel =
        firstSection && firstSection.channels && firstSection.channels.length
          ? firstSection.channels[0]
          : null
      this.activeChannel = firstChannel || null
    },
    onSelectChannel(channel) {
      this.activeChannel = channel
    },
    onJoinVoice(channel) {
      console.log('Join voice', channel)
    },
    onCloseSidebar() {
      this.activeServer = null
      this.activeChannel = null
    },
    handleCreateServer(payload) {
      console.log('Tạo server mới:', payload)
      this.showCreateModal = false
    },
    handleJoinByCode(payload) {
      const code = typeof payload === 'string' ? payload : payload.code
      const nickname = typeof payload === 'string' ? null : payload.nickname
      console.log('Tham gia server bằng mã:', code, 'nickname:', nickname)
      this.showJoinModal = false
    },
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
