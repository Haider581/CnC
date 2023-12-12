function Print(obj) {
	return obj.toString();
}

// This method invokes and returns the output of all the getter methods of a Date object.
function PrintDate(date) {
	var str = new String();
	var dateMethods = new Array(
		"getDay",
		"getFullYear",
		//"getYear", // Deprecated. Use getFullYear() instead.
		"getMonth",
		"getDate",
		"getHours",
		"getMinutes",
		"getSeconds",
		"getMilliseconds",
		"getTime",
		"getTimezoneOffset",
		"getUTCDay",
		"getUTCFullYear",
		"getUTCMonth",
		"getUTCDate",
		"getUTCHours",
		"getUTCMinutes",
		"getUTCSeconds",
		"getUTCMilliseconds",
		//"getVarDate", // IE Only
		"toDateString",
		"toTimeString",
		"toString",
		"toLocaleDateString",
		"toLocaleTimeString",
		"toLocaleString",
		"toUTCString",
		//"toGMTString", // Deprecated. Use toUTCString() instead.
		"valueOf");
	var i;
	try {
		for (i = 0; i < dateMethods.length; i++) {
			str += dateMethods[i] + "() \t -> " + eval("date." + dateMethods[i] + "()") + "\n";
		}
	}
	catch (ex) {
		str = "Error evaluating function: " + dateMethods[i] + "()";
	}
	return str;
}