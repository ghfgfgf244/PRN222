const chatToggle = document.getElementById('chat-toggle');
const chatBox = document.getElementById('chat-box');
const sendBtn = document.getElementById('send-btn');
const userInput = document.getElementById('user-input');
const chatMessages = document.getElementById('chat-messages');

let isChatOpen = false;
let hasInitialized = false;
const conversationHistory = [{
    "role": "user",
    "parts": [{ "text": "Tôi thường xuyên bị đau đầu vào buổi sáng, có khi kèm theo chóng mặt và buồn nôn." }]
},
{
    "role": "model",
    "parts": [{ "text": "Bạn nên khám chuyên khoa Nội tổng quát hoặc Thần kinh để được kiểm tra." }]
},
{
    "role": "user",
    "parts": [{ "text": "Tôi bị nổi mẩn đỏ và ngứa trên da suốt mấy ngày nay." }]
},
{
    "role": "model",
    "parts": [{ "text": "Bạn nên đến khám chuyên khoa Da liễu để được tư vấn và kiểm tra da." }]
},
{
    "role": "user",
    "parts": [{ "text": "Tôi muốn biết cần làm xét nghiệm gì khi bị đau đầu thường xuyên." }]
},
{
    "role": "model",
    "parts": [{ "text": "Bạn có thể cần xét nghiệm máu, đo huyết áp và chụp CT hoặc MRI nếu cần." }]
},
{
    "role": "user",
    "parts": [{ "text": "Tôi hay đau đầu buổi sáng và buồn nôn" }]
},
{
    "role": "model",
    "parts": [{ "text": "Bạn nên khám Nội tổng quát hoặc Thần kinh." }]
},
{
    "role": "user",
    "parts": [{ "text": "Tôi muốn biết cần làm xét nghiệm gì cho triệu chứng đó" }]
},
{
    "role": "model",
    "parts": [{ "text": "Bạn có thể cần khám lâm sàng, xét nghiệm máu, chụp CT hoặc MRI não." }]
},
{
    "role": "user",
    "parts": [{ "text": "Hôm nay thời tiết thế nào?" }]
},
{
    "role": "model",
    "parts": [{ "text": "Tôi chỉ hỗ trợ tư vấn về sức khỏe và bệnh lý." }]
}];

chatToggle.addEventListener('click', () => {
    isChatOpen = !isChatOpen;
    chatBox.style.display = isChatOpen ? 'flex' : 'none';

    if (!hasInitialized && isChatOpen) {
        appendMessage("Xin chào, tôi có thể hỗ trợ bạn tư vấn về sức khỏe. Bạn đang gặp triệu chứng gì hoặc cần hỗ trợ gì về bệnh lý?", "gemini");
        hasInitialized = true;
    }
});

sendBtn.addEventListener('click', sendMessage);
userInput.addEventListener('keypress', (e) => {
    if (e.key === 'Enter') sendMessage();
});

function appendMessage(text, sender, isTyping = false) {
    const bubble = document.createElement('div');
    bubble.className = `chat-bubble ${sender}`;
    bubble.textContent = text;

    if (isTyping) bubble.classList.add('typing');

    bubble.addEventListener('click', () => {
        if (bubble.style.whiteSpace === 'nowrap') {
            bubble.style.whiteSpace = 'normal';
        } else {
            bubble.style.whiteSpace = 'nowrap';
            bubble.style.overflow = 'hidden';
            bubble.style.textOverflow = 'ellipsis';
        }
    });

    chatMessages.appendChild(bubble);
    chatMessages.scrollTop = chatMessages.scrollHeight;
    return bubble;
}

async function sendMessage() {
    const input = userInput.value.trim();
    if (!input) return;

    appendMessage(input, "user");
    userInput.value = "";

    const typingBubble = appendMessage("Đang nhập...", "gemini", true);

    // Nhắc lại prompt định hướng mỗi lần
    const contents = [
        {
            role: "user",
            parts: [{
                "text": "Bạn là trợ lý y tế ảo. Trả lời ngắn gọn, không dùng định dạng (*, markdown). Khi người dùng mô tả triệu chứng, chỉ gợi ý chuyên khoa. Nếu người dùng hỏi rõ về xét nghiệm thì mới gợi ý tên phương pháp kiểm tra tương ứng (lấy từ danh sách sau). Không được giải thích dài dòng. Nếu người dùng hỏi ngoài bệnh lý thì từ chối lịch sự. Dưới đây là dữ liệu các dịch vụ xét nghiệm theo chuyên khoa:\n\n[{\"name\":\"Khám da tổng quát\",\"specialtyId\":1},{\"name\":\"Phân tích dị ứng da\",\"specialtyId\":1},{\"name\":\"Khám mụn trứng cá\",\"specialtyId\":1},{\"name\":\"Điều trị sẹo bằng laser\",\"specialtyId\":1},{\"name\":\"Khám nấm da\",\"specialtyId\":1},{\"name\":\"Phân tích da kỹ thuật số\",\"specialtyId\":1},{\"name\":\"Nội soi tai mũi họng\",\"specialtyId\":2},{\"name\":\"Test dị ứng tai mũi họng\",\"specialtyId\":2},{\"name\":\"Chụp CT tai xương đá\",\"specialtyId\":2},{\"name\":\"Đo thính lực\",\"specialtyId\":2},{\"name\":\"Khám tổng quát\",\"specialtyId\":3},{\"name\":\"Xét nghiệm máu cơ bản\",\"specialtyId\":3},{\"name\":\"Xét nghiệm nước tiểu\",\"specialtyId\":3},{\"name\":\"Đo huyết áp\",\"specialtyId\":3},{\"name\":\"Khám gan mật\",\"specialtyId\":3},{\"name\":\"Khám dạ dày\",\"specialtyId\":3},{\"name\":\"Chụp X-quang xương\",\"specialtyId\":4},{\"name\":\"Đo mật độ xương\",\"specialtyId\":4},{\"name\":\"Khám thoái hóa khớp\",\"specialtyId\":4},{\"name\":\"Chụp MRI cột sống\",\"specialtyId\":4},{\"name\":\"Khám đau lưng – cột sống\",\"specialtyId\":4},{\"name\":\"Điện cơ (EMG)\",\"specialtyId\":4},{\"name\":\"Điện tâm đồ (ECG)\",\"specialtyId\":5},{\"name\":\"Siêu âm tim\",\"specialtyId\":5},{\"name\":\"Đo Holter ECG\",\"specialtyId\":5},{\"name\":\"Test gắng sức\",\"specialtyId\":5},{\"name\":\"Siêu âm Doppler tim\",\"specialtyId\":5},{\"name\":\"Khám tăng huyết áp\",\"specialtyId\":5},{\"name\":\"Tư vấn phát triển trẻ em\",\"specialtyId\":6},{\"name\":\"Khám nhi tổng quát\",\"specialtyId\":6},{\"name\":\"Tiêm chủng định kỳ\",\"specialtyId\":6},{\"name\":\"Tư vấn dinh dưỡng cho trẻ\",\"specialtyId\":6},{\"name\":\"Đo chiều cao – cân nặng\",\"specialtyId\":6},{\"name\":\"Khám sốt siêu vi\",\"specialtyId\":6},{\"name\":\"Khám phụ khoa\",\"specialtyId\":7},{\"name\":\"Siêu âm thai\",\"specialtyId\":7},{\"name\":\"Khám thai định kỳ\",\"specialtyId\":7},{\"name\":\"Xét nghiệm PAP smear\",\"specialtyId\":7},{\"name\":\"Nội soi cổ tử cung\",\"specialtyId\":7},{\"name\":\"Khám vô sinh nữ\",\"specialtyId\":7}]\n\nCác chuyên khoa: Da liễu, Tai mũi họng, Nội tổng quát, Xương khớp, Tim mạch, Nhi khoa, Sản phụ khoa."
            }]
        },
        ...conversationHistory,
        { role: "user", parts: [{ text: input }] }
    ];

    try {
        const response = await fetch("https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=" + API_KEY, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ contents })
        });

        const data = await response.json();
        const reply = data?.candidates?.[0]?.content?.parts?.[0]?.text || "Xin lỗi, tôi không hiểu.";

        chatMessages.removeChild(typingBubble);
        appendMessage(reply, "gemini");

        // ✅ Chỉ lưu nếu câu trả lời ngắn gọn
        if (reply.length < 200) {
            conversationHistory.push(
                { role: "user", parts: [{ text: input }] },
                { role: "model", parts: [{ text: reply }] }
            );
        }

    } catch (err) {
        console.error(err);
        chatMessages.removeChild(typingBubble);
        appendMessage("Có lỗi xảy ra khi gọi Gemini.", "gemini");
    }
}