
/* ------------- this is like a global variable -------------- */
let bookingData = "";

/* --------------------------------- booking modal ------------------------- */

let bookingIdToComplete = null;

/* ------------ booking modal--> open ----------- */

function updateBookingStatus(id) {

    bookingIdToComplete = id;
    const modal = document.getElementById("completeModal");

    if (modal) { modal.classList.remove("hidden"); }
}

/* ------------ booking modal--> close ----------- */

function closeModal() {

    const modal = document.getElementById("completeModal");

    if (modal) { modal.classList.add("hidden"); }

    const confirmBtn = document.getElementById("confirmComplete");

    if (confirmBtn) {
        confirmBtn.disabled = false;
        confirmBtn.innerText = "Complete";
    }

    bookingIdToComplete = null;
}

/* ------------------------------------- Booking Completion logic ----------------------------- */

document.addEventListener("DOMContentLoaded", function () {

    const cancelBtn = document.getElementById("cancelComplete");
    const confirmBtn = document.getElementById("confirmComplete");
    const modal = document.getElementById("completeModal");

    // Cancel button
    if (cancelBtn) { cancelBtn.addEventListener("click", function () { closeModal(); }); }

    // Confirm button
    if (confirmBtn) {

        confirmBtn.addEventListener("click", function () {

            if (!bookingIdToComplete) return;

            // Disable button to prevent double click
            confirmBtn.disabled = true;
            confirmBtn.innerText = "Updating...";

            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            fetch("/Booking/UpdateBookingStatus/" + bookingIdToComplete, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "RequestVerificationToken": token
                }
            })
                .then(res => res.json())
                .then(data => {

                    if (data.success) {

                        const badge = document.getElementById("status-badge-" + bookingIdToComplete);

                        if (badge) {
                            badge.innerText = "Completed";
                            badge.className =
                                "inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800 border border-green-200 dark:bg-green-900/50 dark:text-green-200 dark:border-green-800";
                        }

                        const actionCell = document.getElementById("action-cell-" + bookingIdToComplete);

                        if (actionCell) {
                            actionCell.innerHTML =
                                '<span class="text-green-600 dark:text-green-400 font-bold text-xs uppercase tracking-widest"><i class="fa-solid fa-circle-check mr-1"></i>DONE</span>';
                        }

                    }

                    closeModal();

                })
                .catch(error => {

                    console.error("Update failed:", error);
                    closeModal();

                });

        });

    }

    // Close modal when clicking outside
    if (modal) {
        window.addEventListener("click", function (e) {
            if (e.target === modal) { closeModal(); }
        });
    }

    // ------------------------  Add event listeners for the share modal -------------------------------

    const closeShareBtn = document.getElementById("closeShareBtn");
    const shareModal = document.getElementById("shareModal");

    /* ------------ Message Sharing modal--> close -> when button is clicked ----------- */

    if (closeShareBtn) { closeShareBtn.addEventListener("click", function () { shareModal_close(); }); }

    /* ------------ Message Sharing modal--> close -> when outside is clicked ----------- */

    if (shareModal) {
        window.addEventListener("click", function (e) {
            if (e.target === shareModal) { shareModal_close(); }
        });
    }

    /* -------Error Modal --> close ===> when outside is clicked -------------- */

    const errorModal = document.getElementById("errorModal");
    const closeErrorBtn = document.getElementById("closeErrorBtn")

    if (closeErrorBtn) { closeErrorBtn.addEventListener("click", function () { errorModal_close(); }) }

    if (errorModal) {
        window.addEventListener("click", function (e) {
            if (e.target === errorModal) { errorModal_close(); }
        });
    }

    /* ========================     Share Buttons and Function Calling     ================================= */

    const whatsappBtn = document.getElementById("shareWhatsapp");
    const emailBtn = document.getElementById("shareEmail");
    const smsBtn = document.getElementById("shareSMS");
    const telegramBtn = document.getElementById("shareTelegram");
    const shareNativeBtn = document.getElementById("shareNative");
    const copyBtn = document.getElementById("copyShare");
    const downloadPdfBtn = document.getElementById("downloadPdf");
    const printBtn = document.getElementById("sharePrint");

    whatsappBtn.addEventListener("click", function () {

        const msg = createWhatsappMessage(bookingData);

        const isMobile = /Android|iPhone|iPad|iPod/i.test(navigator.userAgent);
        const url = isMobile ? "https://wa.me/?text=" + encodeURIComponent(msg)
            : "https://web.whatsapp.com/send?text=" + encodeURIComponent(msg);

        if (isMobile) { window.open(url, "_blank"); }
        else {
            window.open(url, "WhatsAppShare");
        }
        shareModal_close();
    });

    copyBtn.addEventListener("click", async function () {

        const icon = copyBtn.getElementsByTagName("i")[0];
        const text = copyBtn.getElementsByTagName("span")[0];

        try {
            const msg = createWhatsappMessage(bookingData);
            await navigator.clipboard.writeText(msg);

            copyBtn.disabled = true;

            icon.className = "fa-solid fa-check text-2xl text-green-600";
            text.innerText = "Copied";

            setTimeout(() => {

                copyBtn.disabled = false;

                icon.className = "fa-regular fa-copy text-2xl";
                text.innerText = "Copy";

            }, 2000);

        }
        catch (err) {

            console.error(err);

            icon.className = "fa-solid fa-xmark text-2xl text-red-600";
            text.innerText = "Failed";

            setTimeout(() => {

                icon.className = "fa-regular fa-copy text-2xl";
                text.innerText = "Copy";

            }, 2000);
        }
    })



});

/* ------------ Message Sharing modal--> close function ----------- */

function shareModal_close() {

    const shareModal = document.getElementById("shareModal");

    if (shareModal) { shareModal.classList.add("hidden"); }
}

/* ------------ Message Sharing modal--> open function ----------- */

let shareMessageId = "";
let isShareLoading = false;

async function shareMessageStatus(id) {

    if (isShareLoading) return;

    isShareLoading = true;

    let shareBtn = document.getElementById("share-btn-" + id)

    try {
        shareMessageId = id;

        if (shareBtn) {
            shareBtn.classList.replace("bg-blue-600", "bg-pink-500");
            shareBtn.disabled = true;
            shareBtn.innerHTML = '<i class="fa-solid fa-spinner fa-spin text-xs"></i><span class="text-xs">Loading...</span>';
        }

        const data = await getBookingDetails(shareMessageId)
        // console.log(data);

        if (!data.success) {
            errorModal_open();
            return
        }
        bookingData = data.booking;
        // console.log(bookingData);

        document.getElementById("shareModal")?.classList.remove("hidden");
    }
    catch (err) {
        errorModal_open();
        console.error(err);
    }
    finally {
        isShareLoading = false;

        if (shareBtn) {
            shareBtn.classList.replace("bg-pink-500", "bg-blue-600");
            shareBtn.disabled = false;
            shareBtn.innerHTML = '<i class="fa-solid fa-share"></i><span>Share</span>';
        }
    }
}

/* ----------------- API call for fetching  -------------- */

async function getBookingDetails(bookingId) {

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    const response = await fetch("/Booking/ShareMessage/" + bookingId, {
        headers: {
            "RequestVerificationToken": token
        }
    });
    return await response.json();
}

// --------------- Error Modal ---------------------- 

function errorModal_close() {

    const errorModal = document.getElementById("errorModal");
    if (errorModal) { errorModal.classList.add("hidden"); }
}

function errorModal_open() {

    const errorModal = document.getElementById("errorModal");
    if (errorModal) { errorModal.classList.remove("hidden"); }
}

/* ---------        Booking Message Body          ------------  */

function createWhatsappMessage(b) {

    let date = new Date(b.bookingDate);

    // Add IST offset (+5:30) 
    date.setMinutes(date.getMinutes() + 330);

    const bookingDate = date.toLocaleDateString("en-IN", {
        day: "2-digit",
        month: "long",
        year: "numeric"
    });

    const bookingTime = date.toLocaleTimeString("en-IN", {
        hour: "2-digit",
        minute: "2-digit",
        hour12: true
    });

    let customDate = "";

    if (b.custom_date) {

        let cDate = new Date(b.custom_date);

        // Add IST offset (+5:30)
        cDate.setMinutes(cDate.getMinutes() + 330);

        customDate = cDate.toLocaleDateString("en-IN", {
            day: "2-digit",
            month: "long",
            year: "numeric"
        });
    }
    const pDay = b.preferred_day;
    let preferDay = "";

    switch (pDay) {
        case "Today":
            preferDay = bookingDate;
            break;
        case "Tomorrow": {

            let d = new Date(b.bookingDate);

            // Add IST offset (+5:30)
            d.setMinutes(d.getMinutes() + 330);
            d.setDate(d.getDate() + 1);

            preferDay = d.toLocaleDateString("en-IN", {
                day: "2-digit",
                month: "long",
                year: "numeric"
            });
        }
            break;
        case "DayAfterTomorrow": {

            let d = new Date(b.bookingDate);

            // Add IST offset (+5:30)
            d.setMinutes(d.getMinutes() + 330);
            d.setDate(d.getDate() + 2);

            preferDay = d.toLocaleDateString("en-IN", {
                day: "2-digit",
                month: "long",
                year: "numeric"
            });
        }
            break;
        default:
            preferDay = "";
            break;
    }

    let brand = "";

    switch (b.brand) {

        case "Skip":
            brand = "";
            break;
        case "Others":
            brand = "";
            break;
        default:
            brand = b.brand;
            break;
    }

    return `🛠️ *APPLIANCE SERVICE BOOKING*
---------------------------------------------------------
*📌 COMPLAINT NO.* : ${b.id}
👩🏻‍💼 *CUSTOMER NAME* : ${b.full_name}
📞 *PHONE* : ${b.phone_number}${b.alt_phone_number ? " | " + b.alt_phone_number : ""}
${b.email ? "*📩 Email* : " + b.email + "\n" : ""}
🔖 *PRODUCT DETAILS :*
${brand ? brand + " | " : ""}${b.product_category}(${b.sub_category}) | ${b.warranty_status}
⚠️ *PROBLEM :* ${b.issue_type === "Other" ? "" : "⇒ " + b.issue_type}
${b.issue_description ? b.issue_description + "\n" : ""}
📍*ADDRESS :* ${b.address?.replace(/\r?\n/g, ", ") || ""}
🏙️ *CITY :* ${b.city} | ${b.pin_code}

${b.service_status === "Completed" ? "✅" : "⛔"} *SERVICE STATUS* : ${b.service_status}
📅 *DATE* : ${bookingDate} | ${bookingTime}
⌛ *TIME SLOT* : ${preferDay || customDate} | ${b.time_slot}${b.priority ? " | " + b.priority : ""}
---------------------------------------------------------
☎️ Call customer before visit.`;
}
