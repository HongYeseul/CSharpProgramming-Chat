using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace CSharpProgramming_Chat
{
    public partial class Form1 : Form
    {
        private Network net = null; // 채팅을 처리하는 Network 클래스 변수 선언
        private Thread server_th = null; // 채팅 서버 스레드 선언
        private string my_ip = null; // 자신의 ip 주소를 기록할 변수
        public Form1()
        {
            InitializeComponent();
            net = new Network(this); // Network 클래스 객체 생성
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if(btn_connect.Text == "연결")
            {
                string ip = txt_ip.Text.Trim();
                if(ip == "")
                {
                    MessageBox.Show("아이피 번호를 입력하세요!");
                    return;
                }
                if (!net.Connect(ip))
                {
                    MessageBox.Show("서버 아이피 번호가 틀리거나\r\n 서버가 작동중이지 않습니다.");
                }
                else
                {
                    btn_connect.Text = "접속중..."; //접속 성공시...
                }
            }
            else
            {
                net.Disconnect();
                btn_connect.Text = "연결";
            }
        }

        private void txt_input_KeyDown(object sender, KeyEventArgs e)
        {
            // enter키 눌리면 문자열 메시지가 상대방에게 전송
            if(e.KeyCode == Keys.Enter)
            {
                string msg = txt_input.Text.Trim();
                Add_MSG("[ 본인]" + msg);
                net.Send(msg);
                txt_input.Text = "";
                txt_input.Focus();
            }
        }

        public void Add_MSG(string msg)
        { 
            txt_info.AppendText(msg + "\r\n"); // 채팅 문자열 출력
            txt_info.ScrollToCaret(); // txt_info 텍스트 박스 자동 스크롤
            txt_input.Focus(); // txt_input 텍스트 박스에 초점 맞춤
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            my_ip = net.Get_MyIP(); // 자신의 ip 정보 알아내기
            server_th = new Thread(new ThreadStart(net.ServerStart));
            server_th.Start();
        }

        // 채팅창이 닫힌 후 발생하는 이벤트(프로그램 종료시)
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if(btn_connect.Text == "접속중...")
                {
                    net.Disconnect(); //채팅 서버와 연결되어 있으면 연결 끊기
                }
                else
                {
                    net.ServerStop(); // 채팅 서버 실행 중지

                    if (server_th.IsAlive)
                        server_th.Abort();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message); //예외 메시지 출력
            }
        }
    }
}
