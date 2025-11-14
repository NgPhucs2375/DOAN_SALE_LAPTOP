// ===== KHỐI 1: JAVASCRIPT "THUẦN" (VANILLA JS) =====
// (Chạy ngay khi HTML được "vẽ" (render) xong, rất nhanh)
document.addEventListener('DOMContentLoaded', function () {

    console.log("site.js (v3 - Vanilla) đã tải!");

    // ===== 1. LOGIC ẨN/HIỆN SIDEBAR (Sửa lỗi) =====
    const logoButton = document.querySelector('.logo-container');
    const body = document.body;
    const sidebarStateKey = 'sidebarCollapsedState'; // Chìa khóa để lưu

    if (logoButton) {

        // A. KIỂM TRA TRẠNG THÁI KHI TẢI TRANG
        if (localStorage.getItem(sidebarStateKey) === 'true') {
            body.classList.add('sidebar-collapsed');
        }

        // B. HÀNH ĐỘNG KHI CLICK
        logoButton.addEventListener('click', function () {
            body.classList.toggle('sidebar-collapsed');

            // LƯU TRẠNG THÁI MỚI (Code "tiến hóa" gọn hơn)
            localStorage.setItem(sidebarStateKey, body.classList.contains('sidebar-collapsed'));
        });
    } else {
        console.error("Lỗi: Không tìm thấy '.logo-container'");
    }

    // ===== 2. LOGIC ACCORDION CHO SIDEBAR (Giữ nguyên) =====
    const filterGroups = document.querySelectorAll('.filter-group-title');

    filterGroups.forEach(groupTitle => {
        groupTitle.addEventListener('click', function () {
            const parentGroup = this.parentElement;
            if (!parentGroup) return;
            const content = parentGroup.querySelector('.filter-group-content');
            if (!content) return;

            if (parentGroup.classList.contains('active')) {
                parentGroup.classList.remove('active');
                content.style.maxHeight = null;
            } else {
                parentGroup.classList.add('active');
                content.style.maxHeight = content.scrollHeight + 'px';
            }
        });
    });

    // Tự động mở mục đầu tiên có class 'active'
    const activeGroup = document.querySelector('.sidebar .filter-group.active');
    if (activeGroup) {
        const content = activeGroup.querySelector('.filter-group-content');
        if (content) {
            setTimeout(() => {
                content.style.maxHeight = content.scrollHeight + 'px';
            }, 100);
        }
    }
});


// ===== KHỐI 2: JQUERY (AJAX) =====
// (Chạy khi "bộ não" (brain) jQuery đã sẵn sàng, an toàn 100%)
$(document).ready(function () {

    console.log("site.js (v3 - jQuery) đã sẵn sàng!");

    // ===== 3. LOGIC "TIẾN HÓA" CHO FORM NEWSLETTER (AJAX) =====
    const newsletterForm = $('#newsletterForm');
    const emailInput = $('#newsletterEmailInput');
    const submitButton = $('#newsletterSubmitButton');
    const messageDiv = $('#newsletterMessage');

    if (newsletterForm.length > 0) {

        newsletterForm.on('submit', function (e) {

            e.preventDefault(); // Ngăn trang web tải lại (reload)

            const email = emailInput.val();
            if (!email) return;

            submitButton.text('Đang xử lý...');
            submitButton.prop('disabled', true);

            $.ajax({
                url: '/Home/SubscribeNewsletter', // Gọi Action
                type: 'POST',
                data: { email: email },
                success: function (response) {
                    if (response.success) {
                        messageDiv.text(response.message);
                        messageDiv.css('color', 'var(--bg-main)'); // Màu Be
                        emailInput.hide();
                        submitButton.hide();
                    } else {
                        messageDiv.text(response.message);
                        messageDiv.css('color', 'var(--color-accent)'); // Màu Đỏ
                        submitButton.text('Đăng ký');
                        submitButton.prop('disabled', false);
                    }
                    messageDiv.show();
                },
                error: function () {
                    messageDiv.text('Đã xảy ra lỗi. Vui lòng thử lại.');
                    messageDiv.css('color', 'var(--color-accent)');
                    messageDiv.show();
                    submitButton.text('Đăng ký');
                    submitButton.prop('disabled', false);
                }
            });
        });
    }
});