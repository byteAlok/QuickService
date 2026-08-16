// ==============================
// PRODUCT DATA
// ==============================

const productData = {
    AC: {
        subs: ['Split AC', 'Window AC', 'Cassette AC', 'Tower AC', 'Central AC'],
        brands: ['Voltas', 'LG', 'Samsung', 'Daikin', 'Hitachi', 'Lloyd', 'Blue Star', 'Carrier', 'Mitsubishi', 'Godrej', 'Haier', 'Panasonic', 'O-General', 'Whirlpool'],
        issues: ['Not Cooling', 'Water Leakage', 'Noisy Operation', 'Gas Refill/Leakage', 'Not Turning On', 'Remote Not Working', 'Service/Cleaning (Jet Pump)']
    },
    Refrigerator: {
        subs: ['Single Door', 'Double Door', 'Side by Side', 'Mini Fridge', 'Deep Freezer'],
        brands: ['Samsung', 'LG', 'Whirlpool', 'Godrej', 'Haier', 'Panasonic', 'Bosch', 'Hitachi', 'Kelvinator', 'Voltas Beko'],
        issues: ['Not Cooling', 'Ice Formation (Frost)', 'Water Leakage', 'Compressor Not Working', 'Strange Noise', 'Door Seal Issue', 'Light Not Working']
    },
    WashingMachine: {
        subs: ['Top Load', 'Front Load', 'Semi Automatic'],
        brands: ['LG', 'Samsung', 'Whirlpool', 'IFB', 'Bosch', 'Godrej', 'Haier', 'Panasonic', 'Onida', 'Lloyd'],
        issues: ['Not Spinning/Drying', 'Not Draining Water', 'Water Leakage', 'Not Taking Water', 'Noisy/Vibrating', 'Error Code Displayed', 'Drum Issue']
    },
    Microwave: {
        subs: ['Solo', 'Grill', 'Convection'],
        brands: ['LG', 'Samsung', 'IFB', 'Morphy Richards', 'Godrej', 'Panasonic', 'Bajaj', 'Whirlpool', 'Haier'],
        issues: ['Not Heating', 'Sparking Inside', 'Plate Not Turning', 'Buttons/Touch Not Working', 'Door Lock Issue', 'Dead/Not Turning On']
    },
    TV: {
        subs: ['LED', 'LCD', 'OLED', 'QLED', 'Smart TV'],
        brands: ['Samsung', 'Sony', 'LG', 'Mi (Xiaomi)', 'OnePlus', 'Panasonic', 'Vu', 'TCL', 'Hisense', 'Sansui', 'Thomson', 'Realme'],
        issues: ['No Display (Sound OK)', 'No Sound (Picture OK)', 'Screen Damage/Lines', 'Wifi/Smart Features Issue', 'Dead/Not Turning On', 'HDMI/Port Issue']
    },
    RO: {
        subs: ['RO + UV', 'RO + UV + UF', 'UV Only'],
        brands: ['Kent', 'Aquaguard (Eureka Forbes)', 'Pureit', 'Livpure', 'Blue Star', 'Havells', 'LG', 'AO Smith'],
        issues: ['Filter Change Request', 'TDS Level Issue', 'Water Leakage', 'Motor/Pump Noise', 'Not Turning On', 'Bad Water Taste']
    },
    Geyser: {
        subs: ['Electric Instant', 'Electric Storage', 'Gas Geyser', 'Solar Geyser'],
        brands: ['Bajaj', 'Crompton', 'Racold', 'V-Guard', 'Havells', 'AO Smith', 'Kenstar', 'Venus'],
        issues: ['Not Heating', 'Water Leakage', 'Low Water Pressure', 'Overheating', 'Electric Shock Sensation', 'Making Noise']
    },
    Treadmill: {
        subs: ['Home Treadmill', 'Commercial Treadmill'],
        brands: ['PowerMax', 'Fitkit', 'Cockatoo', 'Durafit', 'Lifelong', 'Welcare', 'Viva Fitness'],
        issues: ['Belt Slipping/Jerking', 'Motor Issue', 'Display Not Working', 'Error Code (E1, E2...)', 'Incline Issue', 'Strange Noise']
    },
    Electrical: {
        subs: ['Wiring', 'Switchboard', 'Fan', 'Light', 'Inverter/Battery'],
        brands: ['Anchor', 'Havells', 'Legrand', 'Polycab', 'Orient', 'Crompton', 'Luminous', 'Exide', 'Microtek'],
        issues: ['Short Circuit', 'Wiring Fault', 'Switch/Socket Replacement', 'Fan Installation/Repair', 'Light Fitting', 'MCB Tripping']
    }
};

// ==============================
// INIT
// ==============================

document.addEventListener("DOMContentLoaded", function () {

    // Set today min date properly (timezone safe)
    const today = new Date();
    today.setMinutes(today.getMinutes() - today.getTimezoneOffset());
    document.getElementById("customDate").min =
        today.toISOString().split("T")[0];

    document.getElementById("bookingForm")
        .addEventListener("submit", handleSubmit);
});


// ==============================
// PRODUCT CHANGE
// ==============================

function handleProductChange() {

    const cat = document.getElementById("productCategory").value;
    const subSelect = document.getElementById("subCategory");
    const brandSelect = document.getElementById("brand");
    const issueSelect = document.getElementById("issueType");

    subSelect.innerHTML = '<option value="">-- Select Sub Category --</option>';
    brandSelect.innerHTML = '<option value="">-- Select Brand --</option>';
    issueSelect.innerHTML = '<option value="">-- Select Issue Type --</option>';

    if (!cat || !productData[cat]) return;

    const data = productData[cat];

    data.subs?.forEach(s => subSelect.add(new Option(s, s)));
    data.brands?.forEach(b => brandSelect.add(new Option(b, b)));
    data.issues?.forEach(i => issueSelect.add(new Option(i, i)));

    brandSelect.add(new Option("Others", "Others"));
    brandSelect.add(new Option("Skip (Don't Know)", "Skip"));
    issueSelect.add(new Option("Other / Not Listed", "Other"));
}


// ==============================
// VALIDATION
// ==============================

function showError(id, show) {
    const el = document.getElementById(id);
    if (el) el.classList.toggle("hidden", !show);
}

function highlightInput(name, error) {
    const el = document.querySelector(`[name="${name}"]`);
    if (el) el.classList.toggle("input-error", error);
}

function validateForm() {

    let isValid = true;

    const fields = [
        { name: "product_category", id: "productError" },
        { name: "brand", id: "brandError" },
        { name: "issue_type", id: "issueTypeError" },
        { name: "full_name", id: "nameError", min: 3 },
        { name: "phone_number", id: "phoneError", regex: /^[0-9]{10}$/ },
        { name: "address", id: "addressError", min: 10 },
        { name: "city", id: "cityError" },
        { name: "pin_code", id: "pinError", regex: /^[0-9]{6}$/ },
        { name: "preferred_day", id: "dayError" },
        { name: "time_slot", id: "timeError" }
    ];

    fields.forEach(f => {

        const el = document.querySelector(`[name="${f.name}"]`);
        let valid = true;

        if (!el || !el.value) valid = false;
        if (f.min && el.value.length < f.min) valid = false;
        if (f.regex && !f.regex.test(el.value)) valid = false;

        showError(f.id, !valid);
        highlightInput(f.name, !valid);

        if (!valid) isValid = false;
    });

    // Warranty
    const warranty = document.querySelector('input[name="warranty_status"]:checked');
    if (!warranty) {
        showError("warrantyError", true);
        isValid = false;
    } else {
        showError("warrantyError", false);
    }

    // Email (optional)
    const email = document.querySelector('[name="email"]');
    if (email.value &&
        !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
        showError("emailError", true);
        highlightInput("email", true);
        isValid = false;
    } else {
        showError("emailError", false);
        highlightInput("email", false);
    }

    // Custom Date Case
    //const day = document.getElementById("preferredDay");
    //if (day.value === "Custom") {
    //    const customDate = document.getElementById("customDate");
    //    if (!customDate.value) {
    //        showError("customDateError", true);
    //        highlightInput("custom_date", true);
    //        isValid = false;
    //    } else {
    //        showError("customDateError", false);
    //        highlightInput("custom_date", false);
    //    }
    //} else {
    //    showError("customDateError", false);
    //    highlightInput("custom_date", false);
    //}

    const day = document.getElementById('preferredDay');
    if (!day.value) {
        showError('dayError', true);
        highlightInput('preferredDay', true);


        isValid = false;
    } else if (day.value === 'Custom') {
        const cDate = document.getElementById('customDate');
        if (!cDate.value) {
            showError('customDateError', true);
            highlightInput('customDate', true);
            isValid = false;
        }
    }

    return isValid;
}


// ==============================
// SUBMIT
// ==============================

function handleSubmit(e) {

    if (!validateForm()) {
        e.preventDefault();
        return;
    }

    const day = document.getElementById("preferredDay");
    const customDate = document.getElementById("customDate");

    if (day.value === "Custom") {

        // Ensure custom_date ka name present ho
        customDate.setAttribute("name", "custom_date");

    } else {

        // Custom select nahi → custom_date server ko na jaye
        customDate.removeAttribute("name");
    }

    const btn = document.querySelector('button[type="submit"]');
    btn.innerHTML =
        '<i class="fa-solid fa-spinner fa-spin mr-2"></i> Processing...';
    btn.disabled = true;
}


// ==============================
// DAY CHANGE
// ==============================

function handleDayChange() {

    const val = document.getElementById('preferredDay').value;
    const customGroup = document.getElementById('customDateGroup');

    customGroup.classList.toggle('hidden', val !== 'Custom');

    if (val !== 'Custom') {
        document.getElementById("customDate").value = "";
    }
}
















    //    // Data populated in next step...
    //const productData = {
    //    'AC': {
    //    subs: ['Split AC', 'Window AC', 'Cassette AC', 'Tower AC', 'Central AC'],
    //brands: ['Voltas', 'LG', 'Samsung', 'Daikin', 'Hitachi', 'Lloyd', 'Blue Star', 'Carrier', 'Mitsubishi', 'Godrej', 'Haier', 'Panasonic', 'O-General', 'Whirlpool'],
    //issues: ['Not Cooling', 'Water Leakage', 'Noisy Operation', 'Gas Refill/Leakage', 'Not Turning On', 'Remote Not Working', 'Service/Cleaning (Jet Pump)']
    //        },
    //'Refrigerator': {
    //    subs: ['Single Door', 'Double Door', 'Side by Side', 'Mini Fridge', 'Deep Freezer'],
    //brands: ['Samsung', 'LG', 'Whirlpool', 'Godrej', 'Haier', 'Panasonic', 'Bosch', 'Hitachi', 'Kelvinator', 'Voltas Beko'],
    //issues: ['Not Cooling', 'Ice Formation (Frost)', 'Water Leakage', 'Compressor Not Working', 'Strange Noise', 'Door Seal Issue', 'Door Seal Issue', 'Light Not Working']
    //        },
    //'WashingMachine': {
    //    subs: ['Top Load', 'Front Load', 'Semi Automatic'],
    //brands: ['LG', 'Samsung', 'Whirlpool', 'IFB', 'Bosch', 'Godrej', 'Haier', 'Panasonic', 'Onida', 'Lloyd'],
    //issues: ['Not Spinning/Drying', 'Not Draining Water', 'Water Leakage', 'Not Taking Water', 'Noisy/Vibrating', 'Error Code Displayed', 'Drum Issue']
    //        },
    //'Microwave': {
    //    subs: ['Solo', 'Grill', 'Convection'],
    //brands: ['LG', 'Samsung', 'IFB', 'Morphy Richards', 'Godrej', 'Panasonic', 'Bajaj', 'Whirlpool', 'Haier'],
    //issues: ['Not Heating', 'Sparking Inside', 'Plate Not Turning', 'Buttons/Touch Not Working', 'Door Lock Issue', 'Dead/Not Turning On']
    //        },
    //'TV': {
    //    subs: ['LED', 'LCD', 'OLED', 'QLED', 'Smart TV'],
    //brands: ['Samsung', 'Sony', 'LG', 'Mi (Xiaomi)', 'OnePlus', 'Panasonic', 'Vu', 'TCL', 'Hisense', 'Sansui', 'Thomson', 'Realme'],
    //issues: ['No Display (Sound OK)', 'No Sound (Picture OK)', 'Screen Damage/Lines', 'Wifi/Smart Features Issue', 'Dead/Not Turning On', 'HDMI/Port Issue']
    //        },
    //'RO': {
    //    subs: ['RO + UV', 'RO + UV + UF', 'UV Only'],
    //brands: ['Kent', 'Aquaguard (Eureka Forbes)', 'Pureit', 'Livpure', 'Blue Star', 'Havells', 'LG', 'AO Smith'],
    //issues: ['Filter Change Request', 'TDS Level Issue', 'Water Leakage', 'Motor/Pump Noise', 'Not Turning On', 'Bad Water Taste']
    //        },
    //'Geyser': {
    //    subs: ['Electric Instant', 'Electric Storage', 'Gas Geyser', 'Solar Geyser'],
    //brands: ['Bajaj', 'Crompton', 'Racold', 'V-Guard', 'Havells', 'AO Smith', 'Kenstar', 'Venus'],
    //issues: ['Not Heating', 'Water Leakage', 'Low Water Pressure', 'Overheating', 'Electric Shock Sensation', 'Making Noise']
    //        },
    //'Treadmill': {
    //    subs: ['Home Treadmill', 'Commercial Treadmill'],
    //brands: ['PowerMax', 'Fitkit', 'Cockatoo', 'Durafit', 'Lifelong', 'Welcare', 'Viva Fitness'],
    //issues: ['Belt Slipping/Jerking', 'Motor Issue', 'Display Not Working', 'Error Code (E1, E2...)', 'Incline Issue', 'Strange Noise']
    //        },
    //'Electrical': {
    //    subs: ['Wiring', 'Switchboard', 'Fan', 'Light', 'Inverter/Battery'],
    //brands: ['Anchor', 'Havells', 'Legrand', 'Polycab', 'Orient', 'Crompton', 'Luminous', 'Exide', 'Microtek'],
    //issues: ['Short Circuit', 'Wiring Fault', 'Switch/Socket Replacement', 'Fan Installation/Repair', 'Light Fitting', 'MCB Tripping']
    //        }
    //    }; 

    //    document.addEventListener('DOMContentLoaded', () => {
    //        const today = new Date().toISOString().split('T')[0];
    //document.getElementById('customDate').min = today;
    //    });

    //function handleProductChange() {
    //        const cat = document.getElementById('productCategory').value;
    //const subGroup = document.getElementById('subCategoryGroup');
    //const subSelect = document.getElementById('subCategory');
    //const brandSelect = document.getElementById('brand');
    //const issueSelect = document.getElementById('issueType');

    //// Reset
    //subSelect.innerHTML = '<option value="">Select Sub Category</option>';
    //subGroup.classList.add('hidden');
    //brandSelect.innerHTML = '<option value="">Select Brand</option>';
    //issueSelect.innerHTML = '<option value="">Select Issue Type</option>';

    //if (cat && productData[cat]) {
    //            const data = productData[cat];

    //            // Sub-categories
    //            if (data.subs && data.subs.length > 0) {
    //    subGroup.classList.remove('hidden');
    //                data.subs.forEach(sub => {
    //    subSelect.add(new Option(sub, sub));
    //                });
    //            }

    //// Brands
    //if (data.brands) {
    //    data.brands.forEach(brand => brandSelect.add(new Option(brand, brand)));
    //brandSelect.add(new Option("Others", "Others"));
    //brandSelect.add(new Option("Skip (Don't Know)", "Skip"));
    //            }

    //// Issues
    //if (data.issues) {
    //    data.issues.forEach(issue => issueSelect.add(new Option(issue, issue)));
    //issueSelect.add(new Option("Other / Not Listed", "Other"));
    //            }
    //        }
    //    }

    //function handleDayChange() {
    //        const val = document.getElementById('preferredDay').value;
    //document.getElementById('customDateGroup').classList.toggle('hidden', val !== 'Custom');
    //    }

    //function handleTimeChange() {
    //        const val = document.getElementById('timeSlot').value;
    //document.getElementById('specificTimeGroup').classList.toggle('hidden', val !== 'Specific');
    //    }

    //function showError(elId, show) {
    //        const el = document.getElementById(elId);
    //if(show) el.classList.remove('hidden');
    //else el.classList.add('hidden');
    //    }

    //function highlightInput(elName, error) {
    //        const el = document.querySelector(`[name="${elName}"]`);
    //if(el) {
    //            if(error) el.classList.add('input-error');
    //else el.classList.remove('input-error');
    //        }
    //    }

    //function validateForm() {
    //    let isValid = true;

    //// Product
    //const cat = document.getElementById('productCategory');
    //if(!cat.value) {showError('productError', true); highlightInput('product_category', true); isValid = false; }
    //else {showError('productError', false); highlightInput('product_category', false); }

    //const brand = document.getElementById('brand');
    //if(!brand.value) {showError('brandError', true); highlightInput('brand', true); isValid = false; }
    //else {showError('brandError', false); highlightInput('brand', false); }

    //const issue = document.getElementById('issueType');
    //if(!issue.value) {showError('issueTypeError', true); highlightInput('issue_type', true); isValid = false; }
    //else {showError('issueTypeError', false); highlightInput('issue_type', false); }

    //// Customer
    //const name = document.querySelector('input[name="full_name"]');
    //if(name.value.length < 3) {showError('nameError', true); highlightInput('full_name', true); isValid = false; }
    //else {showError('nameError', false); highlightInput('full_name', false); }

    //const phone = document.querySelector('input[name="phone_number"]');
    //if(!/^[0-9]{10}$/.test(phone.value)) {showError('phoneError', true); highlightInput('phone_number', true); isValid = false; }
    //else {showError('phoneError', false); highlightInput('phone_number', false); }

    //const altPhone = document.querySelector('input[name="alt_phone_number"]');
    //if(altPhone.value && !/^[0-9]{10}$/.test(altPhone.value)) {showError('altPhoneError', true); highlightInput('alt_phone_number', true); isValid = false; }
    //else {showError('altPhoneError', false); highlightInput('alt_phone_number', false); }

    //const email = document.querySelector('input[name="email"]');
    //if(email.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {showError('emailError', true); highlightInput('email', true); isValid = false; }
    //else {showError('emailError', false); highlightInput('email', false); }

    //const address = document.querySelector('textarea[name="address"]');
    //if(address.value.length < 10) {showError('addressError', true); highlightInput('address', true); isValid = false; }
    //else {showError('addressError', false); highlightInput('address', false); }

    //const city = document.querySelector('select[name="city"]');
    //if(!city.value) {showError('cityError', true); highlightInput('city', true); isValid = false; }
    //else {showError('cityError', false); highlightInput('city', false); }

    //const pin = document.querySelector('input[name="pin_code"]');
    //if(!/^[0-9]{6}$/.test(pin.value)) {showError('pinError', true); highlightInput('pin_code', true); isValid = false; }
    //else {showError('pinError', false); highlightInput('pin_code', false); }

    //// Schedule
    //const day = document.getElementById('preferredDay');
    //if(!day.value) {showError('dayError', true); highlightInput('preferred_day', true); isValid = false; }
    //else if(day.value === 'Custom' && !document.getElementById('customDate').value) {
    //    showError('customDateError', true); highlightInput('custom_date', true); isValid = false;
    //        } else {
    //    showError('dayError', false); showError('customDateError', false);
    //highlightInput('preferred_day', false); highlightInput('custom_date', false);
    //        }

    //const time = document.getElementById('timeSlot');
    //if(!time.value) {showError('timeError', true); highlightInput('time_slot', true); isValid = false; }
    //else if(time.value === 'Specific' && !document.getElementById('specificTime').value) {
    //    showError('specificTimeError', true); highlightInput('specific_time', true); isValid = false;
    //        } else {
    //    showError('timeError', false); showError('specificTimeError', false);
    //highlightInput('time_slot', false); highlightInput('specific_time', false);
    //        }

    //return isValid;
    //    }

    //function submitBooking() {
    //        if(validateForm()) {
    //    // Mock Submit (Mock Staff)
    //    alert("Booking Created Successfully (Staff)!");
    //window.location.href = '/Staff'; // Redirect to staff dashboard
    //        }
    //    }
