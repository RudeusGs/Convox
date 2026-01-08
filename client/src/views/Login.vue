<template>
  <div class="login-page">
    <div class="login-shell">
      <div class="login-side d-none d-md-flex flex-column justify-content-between">
        <div>
          <h1 class="brand-title">Convox</h1>
          <p class="brand-subtitle">
            Nơi bạn và đồng đội trò chuyện, chia sẻ và làm việc cùng nhau.
          </p>

          <div class="fun-tag">
            <span class="dot-online"></span>
            <span>server #team-đừng-read-only-nữa</span>
          </div>

          <div class="fun-chat-card">
            <div class="fun-chat-header">
              <span class="room-hash">#</span>
              <span class="room-name">daily-standup</span>
              <span class="room-pill">đang diễn ra</span>
            </div>

            <div class="fun-chat-body">
              <div class="fun-msg">
                <div class="fun-msg-name">
                  <span class="name">Leader</span>
                  <span class="time">09:00</span>
                </div>
                <div class="fun-msg-text">
                  Hôm nay ai cũng report đúng 2 phút nha 👀
                </div>
              </div>

              <div class="fun-msg">
                <div class="fun-msg-name">
                  <span class="name">Bạn</span>
                  <span class="time">09:01</span>
                </div>
                <div class="fun-msg-text">
                  Em đang… đăng nhập Convox ạ.
                </div>
              </div>

              <div class="fun-msg typing">
                <div class="fun-msg-name">
                  <span class="name">Cả team</span>
                  <span class="time">đang gõ...</span>
                </div>
                <div class="typing-dots">
                  <span></span><span></span><span></span>
                </div>
              </div>
            </div>
          </div>

          <ul class="fun-list">
            <li>💬 Tách room rõ ràng: code, meme, deadline, than thở.</li>
            <li>🎧 Vào phòng voice xong quên tắt mic là chuyện bình thường.</li>
            <li>🌙 Dark mode mặc định, không lo bị chói giữa đêm deploy.</li>
          </ul>
        </div>

        <div class="brand-note">
          <p>Tip nhỏ: dùng cùng một tài khoản cho web và app desktop.</p>
        </div>
      </div>

      <div class="login-main">
        <button type="button" class="back-button" @click="goBack">
          <span class="back-icon">←</span>
          <span>Quay về</span>
        </button>

        <div class="login-card">
          <div class="login-header mb-3">
            <h3 class="mb-1">Đăng nhập</h3>
            <p class="mb-0 small">
              Nhập tên đăng nhập và mật khẩu để tiếp tục.
            </p>
          </div>

          <form @submit.prevent="submitLogin">
            <div class="mb-3">
              <label class="form-label small">Tên đăng nhập</label>
              <input
                v-model="loginForm.userName"
                type="text"
                class="form-control login-input"
                placeholder="Tên đăng nhập"
              />
            </div>

            <div class="mb-2">
              <label class="form-label small">Mật khẩu</label>
              <input
                v-model="loginForm.password"
                type="password"
                class="form-control login-input"
                placeholder="Mật khẩu"
              />
            </div>

            <div class="d-flex justify-content-between align-items-center mb-3">
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="checkbox"
                  id="rememberMe"
                  v-model="loginForm.remember"
                />
                <label class="form-check-label small" for="rememberMe">
                  Ghi nhớ đăng nhập
                </label>
              </div>
              <button type="button" class="link-button small">
                Quên mật khẩu?
              </button>
            </div>

            <div v-if="error" class="text-danger small mb-2">
              {{ error }}
            </div>

            <button type="submit" class="btn btn-primary w-100 btn-main mb-3">
              Đăng nhập
            </button>
          </form>

          <div class="divider">
            <span>hoặc</span>
          </div>

          <div class="social-row">
            <button
              type="button"
              class="social-icon-btn social-google"
              @click="loginWithGoogle"
              aria-label="Đăng nhập với Google"
            >
              <span class="bi bi-google"></span>
            </button>

            <button
              type="button"
              class="social-icon-btn social-facebook"
              @click="loginWithFacebook"
              aria-label="Đăng nhập với Facebook"
            >
              <span class="bi bi-facebook"></span>
            </button>
          </div>

          <div class="signup-row">
            <span class="small">Chưa có tài khoản?</span>
            <button
              type="button"
              class="signup-link small"
              @click="goRegister"
            >
              Đăng ký ngay
            </button>
          </div>
        </div>

        <p class="copyright small">
          © {{ new Date().getFullYear() }} Convox. All rights reserved.
        </p>
      </div>
    </div>
  </div>
</template>

<script>
export default {
  name: 'LoginView',
  data() {
    return {
      error: '',
      loginForm: {
        userName: '',
        password: '',
        remember: false,
      },
    }
  },
  methods: {
    submitLogin() {
      this.error = ''

      if (!this.loginForm.userName || !this.loginForm.password) {
        this.error = 'Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.'
        return
      }

      console.log('Login form:', this.loginForm)
      this.$router.push('/app')
    },
    loginWithGoogle() {
      console.log('Login with Google')
    },
    loginWithFacebook() {
      console.log('Login with Facebook')
    },
    goBack() {
      if (window.history.length > 1) {
        this.$router.back()
      } else {
        this.$router.push('/')
      }
    },
    goRegister() {
      this.$router.push('/register')
    },
  },
}
</script>

<style scoped>
.login-page {
  min-height: 100vh;
  background: #020617;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 16px;
  color: #e5e7eb;
}

.login-shell {
  width: 100%;
  max-width: 960px;
  display: flex;
  gap: 0;
  border-radius: 18px;
  overflow: hidden;
  box-shadow: 0 24px 60px rgba(0, 0, 0, 0.85);
  border: 1px solid #111827;
}

.login-side {
  flex: 1;
  background: #111827;
  padding: 24px 24px 18px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.brand-title {
  font-size: 24px;
  font-weight: 700;
  color: #6366f1;
  margin-bottom: 8px;
}

.brand-subtitle {
  font-size: 14px;
  color: #9ca3af;
  max-width: 260px;
}

.fun-tag {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  margin-top: 12px;
  padding: 4px 10px;
  border-radius: 999px;
  border: 1px solid #1f2937;
  background-color: #020617;
  font-size: 11px;
  color: #e5e7eb;
}

.dot-online {
  width: 6px;
  height: 6px;
  border-radius: 999px;
  background-color: #22c55e;
}

.fun-chat-card {
  margin-top: 14px;
  border-radius: 12px;
  background-color: #020617;
  border: 1px solid #1f2937;
  padding: 10px 12px;
  font-size: 12px;
  color: #e5e7eb;
}

.fun-chat-header {
  display: flex;
  align-items: center;
  gap: 6px;
  margin-bottom: 8px;
}

.room-hash {
  color: #6b7280;
}

.room-name {
  font-weight: 600;
}

.room-pill {
  margin-left: auto;
  font-size: 10px;
  padding: 2px 6px;
  border-radius: 999px;
  background-color: #111827;
  color: #9ca3af;
}

.fun-chat-body {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.fun-msg-name {
  display: flex;
  align-items: baseline;
  gap: 6px;
}

.fun-msg-name .name {
  font-weight: 600;
}

.fun-msg-name .time {
  font-size: 11px;
  color: #6b7280;
}

.fun-msg-text {
  color: #d1d5db;
}

.fun-msg.typing .typing-dots {
  display: inline-flex;
  gap: 3px;
  margin-top: 2px;
}

.typing-dots span {
  width: 4px;
  height: 4px;
  border-radius: 999px;
  background-color: #6b7280;
  animation: typing-bounce 1s infinite ease-in-out;
}

.typing-dots span:nth-child(2) {
  animation-delay: 0.16s;
}

.typing-dots span:nth-child(3) {
  animation-delay: 0.32s;
}

@keyframes typing-bounce {
  0%,
  80%,
  100% {
    transform: translateY(0);
    opacity: 0.4;
  }
  40% {
    transform: translateY(-3px);
    opacity: 1;
  }
}

.fun-list {
  list-style: none;
  padding: 0;
  margin: 14px 0 0;
  font-size: 12px;
  color: #9ca3af;
}

.fun-list li + li {
  margin-top: 4px;
}

.brand-note {
  font-size: 12px;
  color: #9ca3af;
  border-top: 1px solid #1f2937;
  padding-top: 10px;
}

.login-main {
  flex: 1;
  background-color: #020617;
  padding: 18px 20px 12px;
  display: flex;
  flex-direction: column;
}

.back-button {
  align-self: flex-start;
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 13px;
  border-radius: 999px;
  border: 1px solid #4b5563;
  background: transparent;
  color: #e5e7eb;
  padding: 4px 10px;
  cursor: pointer;
  margin-bottom: 10px;
}

.back-button:hover {
  border-color: #6366f1;
  color: #ffffff;
}

.back-icon {
  font-size: 14px;
}

.login-card {
  width: 100%;
  background-color: #0b1120;
  border-radius: 16px;
  border: 1px solid #111827;
  color: #ffffff;
  padding: 18px 18px 16px;
}

.login-header h3 {
  font-size: 20px;
}

.login-header p {
  color: #9ca3af;
}

.login-input {
  background-color: #020617;
  border-radius: 8px;
  border: 1px solid #1f2937;
  color: #ffffff;
  font-size: 14px;
  padding: 8px 10px;
}

.login-input::placeholder {
  color: #6b7280;
}

.login-input:focus {
  background-color: #020617;
  border-color: #6366f1;
  color: #ffffff;
  box-shadow: 0 0 0 1px #6366f1;
  outline: none;
}

.link-button {
  background: none;
  border: none;
  color: #9ca3af;
  padding: 0;
  cursor: pointer;
}

.link-button:hover {
  color: #e5e7eb;
  text-decoration: underline;
}

.btn-main {
  background-color: #6366f1;
  border-color: #6366f1;
  color: #ffffff;
  font-weight: 600;
  border-radius: 999px;
  padding: 8px 0;
}

.btn-main:hover {
  background-color: #4f46e5;
  border-color: #4f46e5;
  color: #ffffff;
}

.divider {
  display: flex;
  align-items: center;
  gap: 12px;
  font-size: 11px;
  color: #9ca3af;
  margin: 8px 0 10px;
}

.divider::before,
.divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background-color: #1f2937;
}

.social-row {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-top: 4px;
}

.social-icon-btn {
  width: 38px;
  height: 38px;
  border-radius: 999px;
  border: none;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  font-size: 18px;
  cursor: pointer;
  transition: transform 0.12s ease, box-shadow 0.12s ease, filter 0.12s ease;
}

.social-google {
  background-color: #ffffff;
  color: #4285f4;
  box-shadow: 0 0 0 1px #e5e7eb;
}

.social-facebook {
  background-color: #1877f2;
  color: #ffffff;
  box-shadow: 0 0 0 1px #1d4ed8;
}

.social-icon-btn:hover {
  transform: translateY(-1px);
  filter: brightness(1.05);
  box-shadow: 0 0 0 1px #6366f1, 0 8px 18px rgba(0, 0, 0, 0.55);
}

.social-icon-btn:active {
  transform: translateY(0);
  box-shadow: 0 0 0 1px #4b5563, 0 4px 10px rgba(0, 0, 0, 0.6);
}

.signup-row {
  margin-top: 10px;
  text-align: center;
  font-size: 15px;
  color: #9ca3af;
}

.signup-link {
  background: none;
  border: none;
  padding: 0 0 0 4px;
  cursor: pointer;
  color: #6366f1;
}

.signup-link:hover {
  text-decoration: underline;
  color: #818cf8;
}

.form-check-input {
  background-color: #020617;
  border-color: #4b5563;
}

.form-check-input:checked {
  background-color: #6366f1;
  border-color: #6366f1;
}

.form-check-label {
  color: #9ca3af;
}

.text-danger {
  color: #f87171 !important;
}

.copyright {
  margin-top: auto;
  text-align: center;
  color: #6b7280;
  font-size: 11px;
}

@media (max-width: 768px) {
  .login-shell {
    max-width: 480px;
  }
  .login-side {
    display: none !important;
  }
  .login-main {
    padding: 18px 16px 12px;
  }
}
</style>
