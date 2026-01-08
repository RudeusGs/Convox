<template>
  <div class="landing-page">
    <div class="bg-grid"></div>
    <div class="bg-strip"></div>

    <header class="landing-header">
      <div class="brand">
        <img :src="logoConvox" alt="Convox logo" class="brand-logo-img" />
        <span class="brand-name">Convox</span>
      </div>

      <nav class="landing-nav">
        <button class="nav-link" @click="scrollToBottom">
          <i class="bi bi-discord me-1"></i> Cộng đồng
        </button>
        <button class="nav-link nav-link-primary" @click="goLogin">
          <i class="bi bi-box-arrow-in-right me-1"></i> Đăng nhập
        </button>
      </nav>
    </header>

    <main class="landing-main">
      <section class="hero-left">
        <div class="tag-row">
          <span class="tag-pill">
            <i class="bi bi-terminal me-1"></i>
            cho dev, designer, lẫn drama nội bộ
          </span>
        </div>

        <h1 class="hero-title">
          Convox
          <span class="hero-highlight">room-based chat</span>
          cho team
        </h1>

        <p class="hero-desc">
          Không còn group chat loạn xạ. Mỗi chủ đề là một room riêng:
          code, họp, mem, feedback, tám chuyện. Gọn gàng, realtime, full dark mode.
        </p>

        <div class="hero-chips">
          <span class="chip">
            <i class="bi bi-code-slash me-1"></i> Dev room
          </span>
          <span class="chip">
            <i class="bi bi-palette me-1"></i> Design room
          </span>
          <span class="chip">
            <i class="bi bi-controller me-1"></i> Chill / game room
          </span>
        </div>

        <div class="hero-actions">
          <button class="btn-primary" @click="goLogin">
            <i class="bi bi-play-fill me-1"></i>
            Vào phòng ngay
          </button>
        </div>

        <div class="hero-meta">
          <div class="meta-item">
            <span class="meta-dot"></span>
            <span>Multi-room, multi-server, full realtime</span>
          </div>
          <div class="meta-item">
            <span class="meta-dot"></span>
            <span>Mỗi room là một không gian riêng cho team của bạn.</span>
          </div>
        </div>
      </section>

      <section class="hero-right">
        <div class="preview-shell">
          <div class="preview-sidebar">
            <div class="preview-logo">
              <img :src="logoConvox" alt="Convox mini logo" />
            </div>
            <div class="preview-dot active"></div>
            <div class="preview-dot"></div>
            <div class="preview-dot"></div>
            <div class="preview-dot small"></div>
          </div>

          <div class="preview-main">
            <div class="preview-topbar">
              <div class="preview-room">
                <span class="hash">#</span>
                <span class="room-name">team-frontend</span>
              </div>
              <div class="preview-status">
                <span class="status-dot"></span>
                <span>online 6 / 8</span>
              </div>
            </div>

            <div class="preview-body">
              <div class="msg-row">
                <div class="avatar">NT</div>
                <div class="msg-block">
                  <div class="msg-head">
                    <span class="name">Nguyen Quan</span>
                    <span class="time">09:21</span>
                  </div>
                  <div class="msg-text">
                    Tối nay làm nốt sidebar server list, mai demo nhé.
                  </div>
                </div>
              </div>

              <div class="msg-row">
                <div class="avatar alt">TL</div>
                <div class="msg-block">
                  <div class="msg-head">
                    <span class="name">Ton Le</span>
                    <span class="time">09:23</span>
                  </div>
                  <div class="msg-text">
                    Hehehe
                  </div>
                </div>
              </div>

              <div class="msg-row ghost">
                <div class="avatar self">YOU</div>
                <div class="msg-block">
                  <div class="msg-head">
                    <span class="name">Bạn</span>
                    <span class="time">đang gõ...</span>
                  </div>
                  <div class="msg-text typing">
                    <span></span><span></span><span></span>
                  </div>
                </div>
              </div>
            </div>

            <div class="preview-input">
              <div class="input-left">
                <i class="bi bi-hash me-1"></i>
                <span>Gửi gì đó cho mọi người...</span>
              </div>
              <div class="input-right">
                <i class="bi bi-paperclip"></i>
                <i class="bi bi-emoji-smile"></i>
                <i class="bi bi-send-fill"></i>
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>

    <section ref="features" class="landing-features">
      <div class="feature-card">
        <div class="feature-icon">
          <i class="bi bi-hash"></i>
        </div>
        <h3>Room theo chủ đề</h3>
        <p>
          Phòng code, phòng họp, phòng meme. Tách bạch mọi thứ, không còn scroll
          mỏi tay tìm tin nhắn cũ.
        </p>
      </div>

      <div class="feature-card">
        <div class="feature-icon">
          <i class="bi bi-door-closed-fill"></i>
        </div>
        <h3>Phòng khoá mật khẩu</h3>
        <p>
          Muốn họp kín, deal lương, hay bàn chuyện “tối mật”? Tạo room riêng và
          đặt mật khẩu trong một nốt nhạc.
        </p>
      </div>

      <div class="feature-card">
        <div class="feature-icon">
          <i class="bi bi-person-plus-fill"></i>
        </div>
        <h3>Tham gia trong 3 giây</h3>
        <p>
          Đăng nhập bằng Google hoặc Facebook. Tập trung vào nói chuyện, không phải
          reset mật khẩu.
        </p>
      </div>

      <div class="feature-card">
        <div class="feature-icon">
          <i class="bi bi-moon-stars-fill"></i>
        </div>
        <h3>Dark mode gốc</h3>
        <p>
          Giao diện tối, né chói mắt, phù hợp làm việc đêm khuya, tăng 200% độ ngầu
          cho cả team.
        </p>
      </div>
    </section>

    <!-- FOOTER -->
    <footer class="landing-footer">
      <span>© {{ new Date().getFullYear() }} Convox. No light mode, ever.</span>
      <span class="footer-right">
        <i class="bi bi-github me-1"></i> v0.1 • Prototype
      </span>
    </footer>
  </div>
</template>

<script>
import logoConvox from '@/assets/logo/logo_Convox.png'

export default {
  name: 'LandingView',
  data() {
    return {
      logoConvox,
    }
  },
  methods: {
    goLogin() {
      this.$router.push('/login')
    },
    scrollToBottom() {
      window.scrollTo({ top: document.body.scrollHeight, behavior: 'smooth' })
    },
  },
}
</script>

<style scoped>
.landing-page {
  position: relative;
  min-height: 100vh;
  background: #020617;
  color: #e5e7eb;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.bg-grid {
  position: absolute;
  inset: 0;
  background-image: linear-gradient(
      to right,
      rgba(15, 23, 42, 0.8) 1px,
      transparent 1px
    ),
    linear-gradient(to bottom, rgba(15, 23, 42, 0.8) 1px, transparent 1px);
  background-size: 40px 40px;
  opacity: 0.4;
  pointer-events: none;
  z-index: 0;
}

.bg-strip {
  position: absolute;
  inset: 0;
  background:
    radial-gradient(
      ellipse at 50% 10%,
      rgba(99, 102, 241, 0.35),
      transparent 60%
    ),
    radial-gradient(
      ellipse at 50% 80%,
      rgba(15, 23, 42, 0.6),
      transparent 70%
    );
  pointer-events: none;
  z-index: 0;
}

.landing-header {
  position: relative;
  z-index: 1;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 18px 40px;
}

.brand {
  display: flex;
  align-items: center;
  gap: 10px;
}

.brand-logo-img {
  width: 40px;
  height: 40px;
  object-fit: contain;
  border-radius: 10px;
  display: block;
}

.brand-name {
  font-weight: 700;
  font-size: 20px;
}

.landing-nav {
  display: flex;
  gap: 10px;
}

.nav-link {
  background: rgba(15, 23, 42, 0.8);
  border: 1px solid #4b5563;
  border-radius: 999px;
  color: #e5e7eb;
  padding: 8px 18px;
  font-size: 14px;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.nav-link:hover {
  border-color: #6366f1;
}

.nav-link-primary {
  border-color: transparent;
  background: linear-gradient(135deg, #6366f1, #4f46e5);
  box-shadow: 0 0 0 1px rgba(129, 140, 248, 0.3),
    0 14px 30px rgba(15, 23, 42, 0.9);
}

.nav-link-primary:hover {
  background: linear-gradient(135deg, #4f46e5, #4338ca);
}

.landing-main {
  position: relative;
  z-index: 1;
  flex: 1;
  display: grid;
  grid-template-columns: minmax(0, 1.2fr) minmax(0, 1fr);
  gap: 32px;
  padding: 24px 40px 20px;
}

.hero-left {
  max-width: 620px;
  align-self: center;
}

.tag-row {
  margin-bottom: 12px;
}

.tag-pill {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  padding: 4px 14px;
  border-radius: 999px;
  border: 1px solid rgba(148, 163, 184, 0.55);
  background-color: rgba(15, 23, 42, 0.9);
  font-size: 11px;
  text-transform: uppercase;
  letter-spacing: 0.08em;
}

.hero-title {
  font-size: 36px;
  font-weight: 800;
  margin-bottom: 12px;
  line-height: 1.15;
}

.hero-highlight {
  display: inline-block;
  position: relative;
  color: #6366f1;
}

.hero-highlight::after {
  content: '';
  position: absolute;
  left: 0;
  right: 0;
  bottom: -3px;
  height: 2px;
  background: linear-gradient(90deg, #6366f1, transparent);
  opacity: 0.9;
}

.hero-desc {
  font-size: 15px;
  color: #9ca3af;
  margin-bottom: 18px;
}

.hero-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin-bottom: 18px;
}

.chip {
  font-size: 12px;
  border-radius: 999px;
  border: 1px solid #1f2937;
  background-color: rgba(15, 23, 42, 0.95);
  padding: 4px 11px;
  display: inline-flex;
  align-items: center;
}

.hero-actions {
  display: flex;
  flex-wrap: wrap;
  gap: 12px;
  margin-bottom: 18px;
}

.btn-primary {
  background: linear-gradient(135deg, #6366f1, #4f46e5);
  border: 1px solid #6366f1;
  border-radius: 999px;
  color: #ffffff;
  padding: 10px 24px;
  font-size: 15px;
  font-weight: 600;
  cursor: pointer;
  display: inline-flex;
  align-items: center;
  gap: 6px;
}

.btn-primary:hover {
  background: linear-gradient(135deg, #4f46e5, #4338ca);
}

.hero-meta {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 12px;
  color: #9ca3af;
}

.meta-item {
  display: flex;
  align-items: center;
  gap: 6px;
}

.meta-dot {
  width: 6px;
  height: 6px;
  border-radius: 999px;
  background-color: #22c55e;
}

.hero-right {
  align-self: center;
  justify-self: end;
}

.preview-shell {
  width: 100%;
  max-width: 420px;
  background: rgba(15, 23, 42, 0.97);
  border-radius: 18px;
  border: 1px solid rgba(148, 163, 184, 0.45);
  display: flex;
  overflow: hidden;
  box-shadow: 0 26px 60px rgba(0, 0, 0, 0.9);
  backdrop-filter: blur(16px);
}

.preview-sidebar {
  width: 66px;
  background-color: #020617;
  border-right: 1px solid #111827;
  padding: 10px 8px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
}

.preview-logo {
  width: 36px;
  height: 36px;
  border-radius: 12px;
  background-color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

.preview-logo img {
  width: 100%;
  height: 100%;
  object-fit: contain;
}

.preview-dot {
  width: 32px;
  height: 32px;
  border-radius: 14px;
  background-color: #111827;
}

.preview-dot.active {
  background-color: #22c55e;
}

.preview-dot.small {
  width: 26px;
  height: 26px;
  border-radius: 12px;
}

.preview-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  padding: 10px 10px 8px;
}

.preview-topbar {
  display: flex;
  align-items: center;
  justify-content: space-between;
  border-bottom: 1px solid #111827;
  padding-bottom: 6px;
  margin-bottom: 6px;
  font-size: 12px;
}

.preview-room {
  display: flex;
  align-items: center;
  gap: 6px;
}

.hash {
  color: #6b7280;
}

.room-name {
  font-weight: 600;
}

.preview-status {
  display: flex;
  align-items: center;
  gap: 4px;
  color: #9ca3af;
}

.status-dot {
  width: 7px;
  height: 7px;
  border-radius: 999px;
  background-color: #22c55e;
}

.preview-body {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 12px;
  margin-bottom: 6px;
}

.msg-row {
  display: flex;
  gap: 6px;
}

.msg-row.ghost {
  opacity: 0.9;
}

.avatar {
  width: 26px;
  height: 26px;
  border-radius: 999px;
  background-color: #111827;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 11px;
  font-weight: 600;
  color: #e5e7eb;
}

.avatar.alt {
  background-color: #0f766e;
}

.avatar.self {
  background-color: #6366f1;
}

.msg-block {
  flex: 1;
}

.msg-head {
  display: flex;
  align-items: baseline;
  gap: 6px;
}

.msg-head .name {
  font-weight: 600;
}

.msg-head .time {
  font-size: 11px;
  color: #6b7280;
}

.msg-text {
  color: #d1d5db;
}

.msg-text.typing {
  display: inline-flex;
  gap: 3px;
  margin-top: 2px;
}

.msg-text.typing span {
  width: 4px;
  height: 4px;
  border-radius: 999px;
  background-color: #6b7280;
  animation: typing-bounce 1s infinite ease-in-out;
}

.msg-text.typing span:nth-child(2) {
  animation-delay: 0.16s;
}
.msg-text.typing span:nth-child(3) {
  animation-delay: 0.32s;
}

@keyframes typing-bounce {
  0%, 80%, 100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  40% {
    transform: translateY(-3px);
    opacity: 1;
  }
}

.preview-input {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #020617;
  border-radius: 999px;
  border: 1px solid #111827;
  padding: 6px 10px;
  font-size: 11px;
  color: #6b7280;
}

.input-left {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.input-right {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  color: #9ca3af;
}

.landing-features {
  position: relative;
  z-index: 1;
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 14px;
  padding: 10px 40px 16px;
}

.feature-card {
  background: rgba(15, 23, 42, 0.95);
  border-radius: 12px;
  border: 1px solid #111827;
  padding: 14px;
  box-shadow: 0 10px 28px rgba(0, 0, 0, 0.6);
  transition: transform 0.15s ease, box-shadow 0.15s ease,
    border-color 0.15s ease;
}

.feature-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 16px 40px rgba(0, 0, 0, 0.75);
  border-color: rgba(99, 102, 241, 0.6);
}

.feature-icon {
  width: 28px;
  height: 28px;
  border-radius: 999px;
  background-color: #020617;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
  color: #6366f1;
}

.feature-card h3 {
  font-size: 15px;
  margin-bottom: 4px;
}

.feature-card p {
  font-size: 13px;
  color: #9ca3af;
}

.landing-footer {
  position: relative;
  z-index: 1;
  padding: 8px 40px 16px;
  font-size: 12px;
  color: #6b7280;
  border-top: 1px solid #020617;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.footer-right {
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

@media (max-width: 1024px) {
  .landing-main {
    grid-template-columns: minmax(0, 1fr);
  }
  .hero-right {
    display: none;
  }
}

@media (max-width: 768px) {
  .landing-header {
    padding-inline: 16px;
  }
  .landing-main {
    padding-inline: 16px;
    padding-top: 12px;
  }
  .landing-features {
    padding-inline: 16px;
  }
  .hero-left {
    max-width: 100%;
  }
}
</style>
