import * as signalR from '@microsoft/signalr'

class SignalRService {
  private connection: signalR.HubConnection | null = null
  private readonly hubUrl = import.meta.env.VITE_API_URL
    ? `${import.meta.env.VITE_API_URL.replace('/api', '')}/chathub`
    : 'https://localhost:7023/chathub'

  public startConnection() {
    const token = localStorage.getItem('token')
    if (!token) return

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(this.hubUrl, {
        accessTokenFactory: () => token,
      })
      .withAutomaticReconnect()
      .build()

    this.connection
      .start()
      .then(() => console.log('SignalR Connected.'))
      .catch((err) => console.error('SignalR Connection Error: ', err))
  }

  public stopConnection() {
    if (this.connection) {
      this.connection.stop()
      this.connection = null
    }
  }

  // Đăng ký nhận tin nhắn
  public onReceiveMessage<T = unknown>(callback: (message: T) => void) {
    if (this.connection) {
      this.connection.off('ReceiveMessage')
      this.connection.on('ReceiveMessage', callback)
    }
  }
}

export const signalRService = new SignalRService()
