
const productData = {
    'AC': {
        subs: ['Split AC', 'Window AC', 'Cassette AC', 'Tower AC', 'Central AC'],
        brands: ['Voltas', 'LG', 'Samsung', 'Daikin', 'Hitachi', 'Lloyd', 'Blue Star', 'Carrier', 'Mitsubishi', 'Godrej', 'Haier', 'Panasonic', 'O-General', 'Whirlpool'],
        issues: ['Not Cooling', 'Water Leakage', 'Noisy Operation', 'Gas Refill/Leakage', 'Not Turning On', 'Remote Not Working', 'Service/Cleaning (Jet Pump)']
    },
    'Refrigerator': {
        subs: ['Single Door', 'Double Door', 'Side by Side', 'Mini Fridge', 'Deep Freezer'],
        brands: ['Samsung', 'LG', 'Whirlpool', 'Godrej', 'Haier', 'Panasonic', 'Bosch', 'Hitachi', 'Kelvinator', 'Voltas Beko'],
        issues: ['Not Cooling', 'Ice Formation (Frost)', 'Water Leakage', 'Compressor Not Working', 'Strange Noise', 'Door Seal Issue', 'Light Not Working']
    },
    'WashingMachine': {
        subs: ['Top Load', 'Front Load', 'Semi Automatic'],
        brands: ['LG', 'Samsung', 'Whirlpool', 'IFB', 'Bosch', 'Godrej', 'Haier', 'Panasonic', 'Onida', 'Lloyd'],
        issues: ['Not Spinning/Drying', 'Not Draining Water', 'Water Leakage', 'Not Taking Water', 'Noisy/Vibrating', 'Error Code Displayed', 'Drum Issue']
    },
    'Microwave': {
        subs: ['Solo', 'Grill', 'Convection'],
        brands: ['LG', 'Samsung', 'IFB', 'Morphy Richards', 'Godrej', 'Panasonic', 'Bajaj', 'Whirlpool', 'Haier'],
        issues: ['Not Heating', 'Sparking Inside', 'Plate Not Turning', 'Buttons/Touch Not Working', 'Door Lock Issue', 'Dead/Not Turning On']
    },
    'TV': {
        subs: ['LED', 'LCD', 'OLED', 'QLED', 'Smart TV'],
        brands: ['Samsung', 'Sony', 'LG', 'Mi (Xiaomi)', 'OnePlus', 'Panasonic', 'Vu', 'TCL', 'Hisense', 'Sansui', 'Thomson', 'Realme'],
        issues: ['No Display (Sound OK)', 'No Sound (Picture OK)', 'Screen Damage/Lines', 'Wifi/Smart Features Issue', 'Dead/Not Turning On', 'HDMI/Port Issue']
    },
    'RO': {
        subs: ['RO + UV', 'RO + UV + UF', 'UV Only'],
        brands: ['Kent', 'Aquaguard (Eureka Forbes)', 'Pureit', 'Livpure', 'Blue Star', 'Havells', 'LG', 'AO Smith'],
        issues: ['Filter Change Request', 'TDS Level Issue', 'Water Leakage', 'Motor/Pump Noise', 'Not Turning On', 'Bad Water Taste']
    },
    'Geyser': {
        subs: ['Electric Instant', 'Electric Storage', 'Gas Geyser', 'Solar Geyser'],
        brands: ['Bajaj', 'Crompton', 'Racold', 'V-Guard', 'Havells', 'AO Smith', 'Kenstar', 'Venus'],
        issues: ['Not Heating', 'Water Leakage', 'Low Water Pressure', 'Overheating', 'Electric Shock Sensation', 'Making Noise']
    },
    'Treadmill': {
        subs: ['Home Treadmill', 'Commercial Treadmill'],
        brands: ['PowerMax', 'Fitkit', 'Cockatoo', 'Durafit', 'Lifelong', 'Welcare', 'Viva Fitness'],
        issues: ['Belt Slipping/Jerking', 'Motor Issue', 'Display Not Working', 'Error Code (E1, E2...)', 'Incline Issue', 'Strange Noise']
    },
    'Electrical': {
        subs: ['Wiring', 'Switchboard', 'Fan', 'Light', 'Inverter/Battery'],
        brands: ['Anchor', 'Havells', 'Legrand', 'Polycab', 'Orient', 'Crompton', 'Luminous', 'Exide', 'Microtek'],
        issues: ['Short Circuit', 'Wiring Fault', 'Switch/Socket Replacement', 'Fan Installation/Repair', 'Light Fitting', 'MCB Tripping']
    }
};


let currentStep = 1;
const totalSteps = 5;

document.addEventListener('DOMContentLoaded', () => {
    const today = new Date().toISOString().split('T')[0];
    document.getElementById('customDate').min = today;
});

function showError(elId, show) {
    const el = document.getElementById(elId);
    if (show) el.classList.remove('hidden');
    else el.classList.add('hidden');
}
function highlightInput(elName, error) {
    const el = document.querySelector(`[name="${elName}"]`) || document.getElementById(elName);
    if (el) {
        if (error) el.classList.add('input-error');
        else el.classList.remove('input-error');
    }
}

function handleProductChange() {
    const cat = document.getElementById('productCategory').value;

    // 1. Sub-Category
    const subGroup = document.getElementById('subCategoryGroup');
    const subSelect = document.getElementById('subCategory');

    // 2. Brand
    const brandSelect = document.getElementById('brand');

    // 3. Issue
    const issueSelect = document.getElementById('issueType');

    // Reset Sub-cat
    subSelect.innerHTML = '<option value="">-- Select Sub Category --</option>';
    //subGroup.classList.add('hidden');

    // Reset Brand
    brandSelect.innerHTML = '<option value="">-- Select Brand --</option>';

    // Reset Issue
    issueSelect.innerHTML = '<option value="">-- Select Issue Type --</option>';

    if (cat && productData[cat]) {
        const data = productData[cat];

        // Populate Sub-categories
        if (data.subs && data.subs.length > 0) {
            //subGroup.classList.remove('hidden');
            data.subs.forEach(sub => {
                const option = document.createElement('option');
                option.value = sub;
                option.textContent = sub;
                subSelect.appendChild(option);
            });
        }

        // Populate Brands
        if (data.brands && data.brands.length > 0) {
            data.brands.forEach(brand => {
                const option = document.createElement('option');
                option.value = brand;
                option.textContent = brand;
                brandSelect.appendChild(option);
            });
            // Common Options
            const others = new Option("Others", "Others");
            const skip = new Option("Skip (Don't Know)", "Skip");
            brandSelect.add(others);
            brandSelect.add(skip);
        }

        // Populate Issues
        if (data.issues && data.issues.length > 0) {
            data.issues.forEach(issue => {
                const option = document.createElement('option');
                option.value = issue;
                option.textContent = issue;
                issueSelect.appendChild(option);
            });
            // Common Options
            const otherIssue = new Option("Other / Not Listed", "Other");
            issueSelect.add(otherIssue);
        }
    } else {
        // Should not happen really if valid selection
        const skip = new Option("Skip (Don't Know)", "Skip");
        brandSelect.add(skip);
        issueSelect.add(new Option("Other", "Other"));
    }
}

function handleDayChange() {
    const val = document.getElementById('preferredDay').value;
    document.getElementById('customDateGroup').classList.toggle('hidden', val !== 'Custom');
}

//function handleTimeChange() {
//    const val = document.getElementById('timeSlot').value;
//    document.getElementById('specificTimeGroup').classList.toggle('hidden', val !== 'Specific');
//}


//const modal = document.getElementById('visitFeesModal');
//function openModal() { modal.style.display = "flex"; }
//function closeModal() { modal.style.display = "none"; }
//window.onclick = e => { if (e.target == modal) closeModal(); }

function updateUI() {
    document.querySelectorAll('.form-step').forEach(step => step.classList.remove('active'));
    document.getElementById(`step${currentStep}`).classList.add('active');

    const progress = ((currentStep - 1) / (totalSteps - 1)) * 100;
    document.getElementById('progressLine').style.width = `${progress}%`;

    document.querySelectorAll('.step-circle').forEach((circle, idx) => {
        const stepNum = idx + 1;
        circle.classList.remove('active', 'completed');
        circle.innerHTML = stepNum;
        if (stepNum < currentStep) {
            circle.classList.add('completed');
            circle.innerHTML = '<i class="fa-solid fa-check"></i>';
        } else if (stepNum === currentStep) circle.classList.add('active');
    });

    document.getElementById('prevBtn').classList.toggle('hidden', currentStep === 1);
    if (currentStep === totalSteps) {
        document.getElementById('nextBtn').classList.add('hidden');
        document.getElementById('submitBtn').classList.remove('hidden');
    } else {
        document.getElementById('nextBtn').classList.remove('hidden');
        document.getElementById('submitBtn').classList.add('hidden');
    }
    window.scrollTo({ top: 100, behavior: 'smooth' });
}

function validateCurrentStep() {
    let isValid = true;

    // Clear all errors
    document.querySelectorAll('.text-red-500').forEach(e => e.classList.add('hidden'));
    document.querySelectorAll('.input-error').forEach(e => e.classList.remove('input-error'));

    // Validation Logic
    if (currentStep === 1) {
        const cat = document.getElementById('productCategory');
        if (!cat.value) {
            showError('productError', true);
            highlightInput('productCategory', true);
            isValid = false;
        }
        const subGroup = document.getElementById('subCategoryGroup');
        const sub = document.getElementById('subCategory');
        if (!subGroup.classList.contains('hidden') && !sub.value) {
            showError('subCategoryError', true);
            highlightInput('subCategory', true);
            isValid = false;
        }
    }

    if (currentStep === 2) {
        const warranty = document.querySelector('input[name="warranty_status"]:checked');
        if (!warranty) {
            showError('warrantyError', true);
            isValid = false;
        }
    }

    if (currentStep === 3) {
        const brand = document.getElementById('brand');
        if (!brand.value) {
            showError('brandError', true);
            highlightInput('brand', true);
            isValid = false;
        }
        const issue = document.getElementById('issueType');
        if (!issue.value) {
            showError('issueTypeError', true);
            highlightInput('issueType', true);
            isValid = false;
        }
    }

    if (currentStep === 4) {
        const name = document.querySelector('input[name="full_name"]');
        if (name.value.length < 3) {
            showError('nameError', true);
            highlightInput('full_name', true);
            isValid = false;
        }
        const phone = document.querySelector('input[name="phone_number"]');
        if (!/^[0-9]{10}$/.test(phone.value)) {
            showError('phoneError', true);
            highlightInput('phone_number', true);
            isValid = false;
        }
        const altPhone = document.querySelector('input[name="alt_phone_number"]');
        if (altPhone.value && !/^[0-9]{10}$/.test(altPhone.value)) {
            showError('altPhoneError', true);
            highlightInput('alt_phone_number', true);
            isValid = false;
        }
        const email = document.querySelector('input[name="email"]');
        if (email.value && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.value)) {
            showError('emailError', true);
            highlightInput('email', true);
            isValid = false;
        }
        const address = document.querySelector('textarea[name="address"]');
        if (address.value.length < 10) {
            showError('addressError', true);
            highlightInput('address', true);
            isValid = false;
        }
        const city = document.querySelector('select[name="city"]');
        if (!city.value) {
            showError('cityError', true);
            highlightInput('city', true);
            isValid = false;
        }
        const pin = document.querySelector('input[name="pin_code"]');
        if (!/^[0-9]{6}$/.test(pin.value)) {
            showError('pinError', true);
            highlightInput('pin_code', true);
            isValid = false;
        }
    }

    if (currentStep === 5) {
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

        const time = document.getElementById('timeSlot');
        if (!time.value) {
            showError('timeError', true);
            highlightInput('timeSlot', true);
            isValid = false;
        }
        //else if (time.value === 'Specific') {
        //    const sTime = document.getElementById('specificTime');
        //    if (!sTime.value) {
        //        showError('specificTimeError', true);
        //        highlightInput('specificTime', true);
        //        isValid = false;
        //    }
        //}

        const terms = document.getElementById('terms');
        if (!terms.checked) {
            showError('termsError', true);
            isValid = false;
        }
    }

    return isValid;
}

function nextStep() {
    if (validateCurrentStep()) {
        if (currentStep < totalSteps) {
            currentStep++;
            updateUI();
        }
    }
}
function prevStep() { if (currentStep > 1) { currentStep--; updateUI(); } }
function goToStep(step) { if (step < currentStep) { currentStep = step; updateUI(); } }


document.addEventListener("DOMContentLoaded", function () {

    document.getElementById('bookingForm').addEventListener('submit', function (e) {

        if (!validateCurrentStep()) {
            e.preventDefault();
            return;
        }

        const btn = document.getElementById('submitBtn');
        btn.innerHTML = '<i class="fa-solid fa-spinner fa-spin mr-2"></i> Processing...';
        btn.disabled = true;

    });

});
