$.extend($.validity.messages, {
	require: "#{field} هذا الحقل مطلوب.",

	// Format validators:
	match: "#{field} خطاء فى الصيغة.",
	integer: "#{field} يجب ان يكون موجب وعدد.",
	date: "#{field} يجب إدخال تاريخ. (mm/dd/yyyy)",
	email: "#{field} يجب إدخال بريد إليكتروني.",
	usd: "#{field} كمبلغ الدولار الأمريكي.",
	url: "#{field} يجب ان يكون رابط موقع.",
	number: "#{field} يجب ان يكون رقم.",
	zip: "#{field} يجب ان يكون رمز بريدي ##### or #####-####.",
	phone: "#{field} يجب ان يكون رقم هاتف ###-###-####.",
	guid: "#{field} يجب ان يكون Guid .",
	time24: "#{field} يجب ان يكون وقت 24 ساعة 23:00.",
	time12: "#{field} يجب ان يكون وقت 12 ساعة 12:00 AM/PM",

	// Value range messages:
	lessThan: "#{field} يجب ان يكون اقل من  #{max}.",
	lessThanOrEqualTo: "#{field} يجب ان يكون اقل من او يساوي #{max}.",
	greaterThan: "#{field} يجب ان يكون أكبر من #{min}.",
	greaterThanOrEqualTo: "#{field} يجب ان يكون اكبر من او يساوي #{min}.",
	range: "#{field} يجب ان ينحصر بين #{min} و #{max}.",

	// Value length messages:
	tooLong: "#{field} يجب ان يكون اقل من #{max} characters.",
	tooShort: "#{field} يجب ان يكون اكبر من #{min} characters.",

	// Composition validators:
	nonHtml: "#{field} cannot contain HTML characters.",
	alphabet: "#{field} contains disallowed characters.",

	minCharClass: "#{field} cannot have more than #{min} #{charClass} characters.",
	maxCharClass: "#{field} cannot have less than #{min} #{charClass} characters.",

	// Aggregate validator messages:
	equal: "القيمة غير متساوية.",
	distinct: "القيمة مكررة.",
	sum: "لا يمكن الجمع #{sum}.",
	sumMax: "The sum of the values must be less than #{max}.",
	sumMin: "The sum of the values must be greater than #{min}.",

	// Radio validator messages:
	radioChecked: "القيمة المحددة غير صحيحة.",

	generic: "خطاء."
});

$.validity.setup({ defaultFieldName: "*", });
