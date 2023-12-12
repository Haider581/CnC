Calendar = new Function();
Calendar.msSecond = 1000;
Calendar.msMinute = Calendar.msSecond * 60;
Calendar.msHour = Calendar.msMinute * 60;
Calendar.msDay = Calendar.msHour * 24;
Calendar.msWeek = Calendar.msDay * 7;
Calendar.msTimezoneOffset = Calendar.msMinute * new Date(0).getTimezoneOffset();
Calendar.supportedTypes = new Array(
	4, "Persian",
	6, "Hijri",
	7, "Buddhist",
	8, "Hebrew",
	16, "SakaEra",
	22, "EastAsian",
	24, "Japanese",
	26, "Julian",
	28, "Korean",
	30, "Taiwan",
	32, "UmAlQura"
	// add more calendar types here
);
Calendar.localType = 1;
Calendar.digitShape = 0;
Calendar.getCalName = CalendarGetCalendarName;
function CalendarGetCalendarName(caltype) {
	var calName = null;
	for (var i = 0; i < Calendar.supportedTypes.length; i+=2) {
		if (caltype === Calendar.supportedTypes[i]) {
			calName = Calendar.supportedTypes[i+1];
			break;
		}
	}
	return calName;
}

// String String.changeDigitShapes(Number shape[, Number num])
// Converts digits in a string or number to any of the Unicode digit representations.
String.prototype.changeDigitShapes = StringChangeDigitShapes;
function StringChangeDigitShapes(shape, num) {
	var sNum, zeros, i, c, z;
	num = (num == null) ? this : num.toString();
	sNum = new String();
	zeros = new Array(0x0030, 0x0660, 0x06F0, 0x0966, 0x09E6, 0x0A66, 0x0AE6, 0x0B66, 0x0BE6, 0x0C66, 0x0CE6, 0x0D66, 0x0E50, 0x0ED0, 0x0F20, 0x1040, 0x17E0, 0x1810, 0x1946, 0x19D0, 0xFF10, 0x104A0);
	for (i = 0; i < num.length; i++) {
		c = num.charCodeAt(i);
		for (z in zeros) {
			z = zeros[z];
			if (z <= c && c <= z + 9) {
				c = c - z + zeros[shape];
				break;
			}
		}
		sNum += String.fromCharCode(c);
	}
	return sNum;
}

// Used to inherit the prototypes (instance methods) of a class
Function.prototype.extend = extendClass;
function extendClass(base) {
	if (typeof(base) === "function") {
		for (var m in base.prototype)
			eval("this.prototype."+m+"=base.prototype."+m+";");
		this.baseClass = base;
		this.prototype.base = function() {
			this.constructor.baseClass.apply(this, this.constructor.arguments);
		}
	}
}

// <IntlDate> is an object that is inheriting from IntlDateBase.

// public
// <IntlDate> Date.localize([caltype])
// caltype:	Calendar type. If not specified, Calendar.localType is used.
// returns:	An <IntlDate> object or this object if caltype equals 1.
Date.prototype.localize = DateLocalize;
function DateLocalize(caltype) {
	if (caltype == null)
		caltype = Calendar.localType;
	if (caltype > 1 || typeof(caltype) === "string")
		return this.toIntlDate(caltype, true);
	else
		return this;
}

// public
// Date Date.delocalize()
Date.prototype.delocalize = DateDelocalize;
function DateDelocalize() {
	return this;
}

// <IntlDate> Date.toIntlDate(caltype[, lateInit])
// caltype: Calendar type
// lateInit: Optional, late initialization. Default is false.
// returns:	An <IntlDate> object. Returns this object if caltype equals 1 or is not supported.
Date.prototype.toIntlDate = DateToIntlDate;
function DateToIntlDate(caltype, lateInit) {
	if (caltype === 1)
		return this;
	var calName = typeof(caltype) === "number" ? Calendar.getCalName(caltype) : caltype.toString();
	if (calName) {
		try {
			return eval("this.to" + calName + "Date(" + !!lateInit + ");");
		}
		catch (ex){
			ex.message = "Cannot invoke method 'Date.prototype.to" + calName + "Date()'.";
			throw ex;
		}
	}
	else
		return this;
}

// public
// <IntlDate> IntlDateBase.localize([caltype])
IntlDateBase.prototype.localize = IntlDateBaseLocalize;
function IntlDateBaseLocalize(caltype) {
	if (caltype != null && caltype != this.caltype) {
		return this.toDate().localize(caltype);
	}
	else
		return this;
}

// public
// Date IntlDateBase.delocalize()
IntlDateBase.prototype.delocalize = IntlDateBaseDelocalize;
function IntlDateBaseDelocalize() {
	return this.toDate();
}

function fix(a) {
	return (a - a % 1);
}

String.prototype.startsWith = function (str) {
	return this.slice(0, str.length) === str;
};

String.prototype.endsWith = function (str) {
	return this.slice(-str.length) === str;
};

// abstract class, base constructor
// new IntlDateBase(year, month, date[, hours[, minutes[, seconds[, caltype]]]])
// Derived classed can be created with the following syntax:
// new LocaleDate(year, month, date[, hours[, minutes[, seconds)
// new LocaleDate(dateObject)
// new LocaleDate(msTicks)
// new LocaleDate()
function IntlDateBase(year, month, day, hr, min, sec, caltype) {
	if (this.constructor.name.startsWith("IntlDate")) { // .ctor called with a new keyword
		if (caltype === undefined) {
			throw new Error("'caltype' not specified in the constructor call.");
		}

		var calName = typeof(caltype) === "number" ? Calendar.getCalName(caltype) : caltype;
		if (calName) {
			try {
				return eval("new " + calName + "Date(year, month, day, hr, min, sec);");
			}
			catch (ex) {
				ex.message = "Could not create object '" + calName + "Date'.";
				throw ex;
			}
		}
		else {
			throw new Error("The specified calendar is not supported.");
		}
	}
	else { // .base() called from a child class
		if (!this.constructor.name.endsWith("Date")) {
			// derived classes of IntlDate must end with 'Date'
			throw new Error("Operation not supported.");
		}
		if (arguments.length === 0) { // use current time
			var msTicks = new Date().getTime();
			this.reset.apply(this, [msTicks]);
			return;
		}
		if (arguments.length === 1) { // first param is a Date object, an IntlDate object, or msTicks
			var param1 = year;
			var msTicks = (param1.getTime === "function") ? param1.getTime() : param1 - 0;
			this.reset.apply(this, [msTicks]);
			return;
		}
		// arguments.length > 1
		this.reset.apply(this, arguments);
	}
}

// void IntlDateBase.reset(year, month, date[, hours[, minutes[, seconds]]])
// void IntlDateBase.reset(msTicks)
IntlDateBase.prototype.reset = IntlDateBaseReset;
function IntlDateBaseReset(year, month, day, hr, min, sec) {
	var msTicks;
	if (arguments.length === 1) {
		msTicks = year - 0;
		year = null;
	}
	this.msTicks = null;
	this.dow = null;
	this.yr = (year) ? fix(year) : 0;
	this.mon = (month) ? fix(month) : 0;
	this.day = (day) ? fix(day) : 0;
	this.hr = (hr) ? fix(hr) : 0;
	this.min = (min) ? fix(min) : 0;
	this.sec = (sec) ? fix(sec) : 0;
	if (msTicks != null)
		this.msTicks = msTicks;
	else
		this.ensureFlat();
}

// All abstract methods have to be overridden in the derived classes.
// Virtual methods can be overridden or use the default implementation in the base class.

// abstract
// Date IntlDateBase.toDate()
IntlDateBase.prototype.toDate = IntlDateBaseToDate;
function IntlDateBaseToDate() {
	throw new Error("toDate() not implemented.");
}

// abstract
// <IntlDate> IntlDateBase.getLocaleDate(Date date)
IntlDateBase.prototype.getLocaleDate = IntlDateGetLocaleDate;
function IntlDateGetLocaleDate(date) {
	throw new Error("getLocaleDate() not implemented.");
}

// abstract
// Boolean IntlDateBase.validRange()
IntlDateBase.prototype.validRange = IntlDateBaseValidRange;
function IntlDateBaseValidRange() {
	throw new Error("validRange() not implemented.");
}

// abstract
// Number IntlDateBase.getDaysInMonth(Number year, Number month)
IntlDateBase.prototype.getDaysInMonth = IntlDateBaseGetDaysInMonth;
function IntlDateBaseGetDaysInMonth(year, month) {
	throw new Error("getDaysInMonth() not implemented.");
}

// abstract
// Boolean IntlDateBase.isLeapYear(Number year)
IntlDateBase.prototype.isLeapYear = IntlDateBaseIsLeapYear;
function IntlDateBaseIsLeapYear(year) {
	throw new Error("isLeapYear() not implemented.");
}

// virtual
// String IntlDateBase.toDateString()
IntlDateBase.prototype.toDateString = IntlDateBaseToDateString;
function IntlDateBaseToDateString()
{
	var str = "Year: " + this.getFullYear() +
		", Mon: " + (this.getMonth()+1) +
		", Day: " + this.getDate();
	return str;
}

// virtual
// String IntlDateBase.toTimeString()
IntlDateBase.prototype.toTimeString = IntlDateBaseToTimeString;
function IntlDateBaseToTimeString()
{
	var str = "hour: " + this.getHours() +
		", min: " + this.getMinutes() +
		", sec: " + this.getSeconds();
	return str;
}

// virtual
// String IntlDateBase.toString()
IntlDateBase.prototype.toString = IntlDateBaseToString;
function IntlDateBaseToString()
{
	return this.toDateString() + "  " +
		   this.toTimeString();
}

// virtual
// String IntlDateBase.toLocaleDateString()
IntlDateBase.prototype.toLocaleDateString = IntlDateBaseToLocaleDateString;
function IntlDateBaseToLocaleDateString()
{
	return this.toDateString()
		.changeDigitShapes(Calendar.digitShape);
}

// virtual
// String IntlDateBase.toLocaleTimeString()
IntlDateBase.prototype.toLocaleTimeString = IntlDateBaseToLocaleTimeString;
function IntlDateBaseToLocaleTimeString()
{
	return this.toTimeString()
		.changeDigitShapes(Calendar.digitShape);
}

// virtual
// String IntlDateBase.toLocaleString()
IntlDateBase.prototype.toLocaleString = IntlDateBaseToLocaleString;
function IntlDateBaseToLocaleString()
{
	return this.toString()
		.changeDigitShapes(Calendar.digitShape);
}

// virtual
// String IntlDateBase.toUTCString()
IntlDateBase.prototype.toUTCString = IntlDateBaseToUTCString;
function IntlDateBaseToUTCString()
{
	this.ensureDecode();
	var str = "Year: " + this.getUTCFullYear() +
		", Mon: " + (this.getUTCMonth() + 1) +
		", Day: " + this.getUTCDate() +
		"\rhour: " + this.getUTCHours() +
		", min: " + this.getUTCMinutes() +
		", sec: " + this.getUTCSeconds();
	return str;
}

// obsolete
// This method is provided for backwards compatibility only.
IntlDateBase.prototype.toGMTString = IntlDateBaseToUTCString;

// String IntlDateBase.getName()
IntlDateBase.prototype.getName = IntlDateBaseGetName;
function IntlDateBaseGetName() {
	return typeof(this.caltype) === "number" ? Calendar.getCalName(this.caltype) : this.caltype.toString();
}

// Number IntlDateBase.valueOf()
IntlDateBase.prototype.valueOf = IntlDateBaseValueOf;
function IntlDateBaseValueOf() {
	var status = 0;
	if (!this.validRange())
		status = Number.NaN;
	return status;
}

// Boolean IntlDateBase.isEmpty()
IntlDateBase.prototype.isEmpty = IntlDateBaseIsEmpty;
function IntlDateBaseIsEmpty() {
	return (!this.yr) && (!this.mon) && (!this.day)
		&& (!this.hr) && (!this.min) && (!this.sec);
}

// void IntlDateBase.copyFrom(IntlDateBase date)
IntlDateBase.prototype.copyFrom = IntlDateBaseCopyFrom;
function IntlDateBaseCopyFrom(date) {
	this.reset(date.yr, date.mon, date.day,
			   date.hr, date.min, date.sec);
	this.msTicks = date.msTicks;
	this.dow = date.dow;
}

// <IntlDate> IntlDateBase.clone()
IntlDateBase.prototype.clone = IntlDateBaseClone;
function IntlDateBaseClone() {
	var date = new this.constructor(this.yr, this.mon, this.day, this.hr, this.min, this.sec);
	this.ensureTime();
	date.msTicks = this.msTicks;
	date.dow = this.dow;
	return date;
}

// void IntlDateBase.ensureDecode()
IntlDateBase.prototype.ensureDecode = IntlDateBaseEnsureDecode;
function IntlDateBaseEnsureDecode() {
	if (this.isEmpty() && this.msTicks !== null) {
		var date = new Date(this.msTicks);
		date = this.getLocaleDate(date);
		this.copyFrom(date);
	}
}

// void IntlDateBase.ensureTime()
IntlDateBase.prototype.ensureTime = IntlDateBaseEnsureTime;
function IntlDateBaseEnsureTime() {
	if (!this.isEmpty() && !this.msTicks) {
		var date = this.toDate();
		if (date) {
			this.msTicks = date.getTime();
			this.dow = date.getUTCDay();
		}			
	}
}

// void IntlDateBase.ensureFlat()
IntlDateBase.prototype.ensureFlat = IntlDateBaseEnsureFlat;
function IntlDateBaseEnsureFlat() {
	if (this.validRange())
		return;
	var year, month, day, hr, min, sec, msHours, timeOnly, yrOff;
	year = fix(this.yr);
	month = fix(this.mon);
	day = fix(this.day);
	hr = fix(this.hr);
	min = fix(this.min);
	sec = fix(this.sec);
	msHours = new Date(0).setHours(hr, min, sec, 0);
	timeOnly = new Date(msHours);
	hr = timeOnly.getHours();
	min = timeOnly.getMinutes();
	sec = timeOnly.getSeconds();
	day += Math.floor(msHours / Calendar.msDay);
	yrOff = Math.floor(month / 12);
	year += yrOff;
	month -= yrOff * 12;
	this.reset(year, month, 1, hr, min, sec);
	this.setDate(day);
}

IntlDateBase.prototype.getTimezoneOffset = IntlDateBaseGetTimezoneOffset;
function IntlDateBaseGetTimezoneOffset() {
	return new Date(0).getTimezoneOffset();
}

IntlDateBase.prototype.getDay = IntlDateBaseGetDay;
function IntlDateBaseGetDay() {
	this.ensureDecode();
	this.ensureTime();
	return this.dow;
}

// obsolete
// This method is provided for backwards compatibility only.
IntlDateBase.prototype.getYear = IntlDateBaseGetFullYear;

IntlDateBase.prototype.getFullYear = IntlDateBaseGetFullYear;
function IntlDateBaseGetFullYear() {
	this.ensureDecode();
	return this.yr;
}

IntlDateBase.prototype.getMonth = IntlDateBaseGetMonth;
function IntlDateBaseGetMonth() {
	this.ensureDecode();
	return this.mon;
}

IntlDateBase.prototype.getDate = IntlDateBaseGetDate;
function IntlDateBaseGetDate() {
	this.ensureDecode();
	return this.day;
}

IntlDateBase.prototype.getHours = IntlDateBaseGetHours;
function IntlDateBaseGetHours() {
	this.ensureDecode();
	return this.hr;
}

IntlDateBase.prototype.getMinutes = IntlDateBaseGetMinutes;
function IntlDateBaseGetMinutes() {
	this.ensureDecode();
	return this.min;
}

IntlDateBase.prototype.getSeconds = IntlDateBaseGetSeconds;
function IntlDateBaseGetSeconds() {
	this.ensureDecode();
	return this.sec;
}

IntlDateBase.prototype.getMilliseconds = IntlDateBaseGetMilliseconds;
function IntlDateBaseGetMilliseconds() {
	return 0;
}

IntlDateBase.prototype._getZeroOffsetDate = IntlDateBaseGetZeroOffsetDate;
function IntlDateBaseGetZeroOffsetDate(native) {
	var utcDate = new Date(this.getTime() + Calendar.msTimezoneOffset);
	return native ? utcDate : utcDate.toIntlDate(this.caltype, false);
}

IntlDateBase.prototype.getUTCDay = IntlDateBaseGetUTCDay;
function IntlDateBaseGetUTCDay() {
	return this._getZeroOffsetDate().getDay();
}

IntlDateBase.prototype.getUTCFullYear = IntlDateBaseGetUTCFullYear;
function IntlDateBaseGetUTCFullYear() {
	return this._getZeroOffsetDate().getFullYear();
}

IntlDateBase.prototype.getUTCMonth = IntlDateBaseGetUTCMonth;
function IntlDateBaseGetUTCMonth() {
	return this._getZeroOffsetDate().getMonth();
}

IntlDateBase.prototype.getUTCDate = IntlDateBaseGetUTCDate;
function IntlDateBaseGetUTCDate() {
	return this._getZeroOffsetDate().getDate();
}

IntlDateBase.prototype.getUTCHours = IntlDateBaseGetUTCHours;
function IntlDateBaseGetUTCHours() {
	return this._getZeroOffsetDate(true).getHours();
}

IntlDateBase.prototype.getUTCMinutes = IntlDateBaseGetUTCMinutes;
function IntlDateBaseGetUTCMinutes() {
	return this._getZeroOffsetDate(true).getMinutes();
}

IntlDateBase.prototype.getUTCSeconds = IntlDateBaseGetSeconds;

IntlDateBase.prototype.getUTCMilliseconds = IntlDateBaseGetMilliseconds;

// deprecated, IE Only
// This method is provided for backwards compatibility only.
if (Date.prototype.getVarDate)
	IntlDateBase.prototype.getVarDate = function() {
	return this.toDate().getVarDate();
}

IntlDateBase.prototype.getTime = IntlDateBaseGetTime;
function IntlDateBaseGetTime() {
	this.ensureTime();
	return this.msTicks;
}

IntlDateBase.prototype.setTime = IntlDateBaseSetTime;
function IntlDateBaseSetTime(msTicks) {
	this.reset(msTicks);
}

// obsolete
// This method is provided for backwards compatibility only.
IntlDateBase.prototype.setYear = IntlDateBaseSetFullYear;

IntlDateBase.prototype.setFullYear = IntlDateBaseSetFullYear;
function IntlDateBaseSetFullYear(year, month, date) {
	this.ensureDecode();
	if (year != null) {
		var yr = fix(year);
		if (this.yr != yr) {
			this.yr = yr;
			this.msTicks = null;
		}
	}
	if (month != null) {
		var mon = fix(month);
		if (this.mon != mon) {
			this.mon = mon;
			this.msTicks = null;
		}
	}
	if (date != null) {
		var day = fix(date);
		if (this.day != day) {
			this.day = day;
			this.msTicks = null;
		}
	}
	this.ensureFlat();
}

IntlDateBase.prototype.setMonth = IntlDateBaseSetMonth;
function IntlDateBaseSetMonth(month, date) {
	this.setFullYear(null, month, date);
}

IntlDateBase.prototype.setDate = IntlDateBaseSetDate;
function IntlDateBaseSetDate(date) {
	var curDate, delta;
	curDate = this.getDate();
	delta = fix(date) - curDate;
	if (delta != 0)
		this.setTime(this.getTime() + delta * Calendar.msDay);
}

IntlDateBase.prototype.setHours = IntlDateBaseSetHours;
function IntlDateBaseSetHours(hr, min, sec, msec) {
	var msCurHours, time, delta;
	msCurHours = this.getTime() % Calendar.msDay;
	if (hr == null) hr = msCurHours / Calendar.msHour;
	if (min == null) min = (msCurHours % Calendar.msHour) / Calendar.msMinute;
	if (sec == null) sec = (msCurHours % Calendar.msMinute) / Calendar.msSecond;
	if (msec == null) msec = msCurHours % Calendar.msSecond;
	hr = fix(hr);
	min = fix(min);
	sec = fix(sec);
	msec = fix(msec);
	time = new Date(0);
	time.setHours(hr, min, sec, msec);
	delta = time.getTime() - msCurHours;
	if (delta != 0)
		this.setTime(this.getTime() + delta);
}

IntlDateBase.prototype.setMinutes = IntlDateBaseSetMinutes;
function IntlDateBaseSetMinutes(min) {
	min = fix(min);
	this.setHours(null, min);
}

IntlDateBase.prototype.setSeconds = IntlDateBaseSetSeconds;
function IntlDateBaseSetSeconds(sec) {
	sec = fix(sec);
	this.setHours(null, null, sec);
}

IntlDateBase.prototype.setMilliseconds = IntlDateBaseSetMilliseconds;
function IntlDateBaseSetMilliseconds(msec) {
	msec = fix(msec);
	this.setHours(null, null, null, msec);
}

IntlDateBase.prototype.setUTCFullYear = IntlDateBaseSetUTCFullYear;
function IntlDateBaseSetUTCFullYear(year, month, date) {
	var utcDate = this._getZeroOffsetDate();
	utcDate.setFullYear(year, month, date);
	this.setTime(utcDate.getTime() - Calendar.msTimezoneOffset);
}

IntlDateBase.prototype.setUTCMonth = IntlDateBaseSetUTCMonth;
function IntlDateBaseSetUTCMonth(month, date) {
	this.ensureFlat(); var utcDate = this._getZeroOffsetDate();
	utcDate.setMonth(month, date);
	this.setTime(utcDate.getTime() - Calendar.msTimezoneOffset);
}

IntlDateBase.prototype.setUTCDate = IntlDateBaseSetUTCDate;
function IntlDateBaseSetUTCDate(date) {
	var utcDate = this._getZeroOffsetDate();
	utcDate.setDate(date);
	this.setTime(utcDate.getTime() - Calendar.msTimezoneOffset);
}

IntlDateBase.prototype.setUTCHours = IntlDateBaseSetUTCHours;
function IntlDateBaseSetUTCHours(hr, min, sec, msec) {
	var utcDate = this._getZeroOffsetDate();
	utcDate.setHours(hr, min, sec, msec);
	this.setTime(utcDate.getTime() - Calendar.msTimezoneOffset);
}

IntlDateBase.prototype.setUTCMinutes = IntlDateBaseSetUTCMinutes;
function IntlDateBaseSetUTCMinutes(min) {
	var utcDate = this._getZeroOffsetDate();
	utcDate.setMinutes(min);
	this.setTime(utcDate.getTime() - Calendar.msTimezoneOffset);
}

IntlDateBase.prototype.setUTCSeconds = IntlDateBaseSetSeconds;

IntlDateBase.prototype.setUTCMilliseconds = IntlDateBaseSetMilliseconds;

IntlDate = IntlDateBase;