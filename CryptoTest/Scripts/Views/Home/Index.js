
// The real code

var message = "SuperSecret!!";

var bytesInSalt = 128 / 8;
var salt = CryptoJS.lib.WordArray.random(bytesInSalt);

var wordArraySubArray = function (array, index, length) {
    var hex = array.toString(CryptoJS.enc.Hex);
    var subHex = hex.substr(index * 2, length * 2);
    return CryptoJS.enc.Hex.parse(subHex);
};

var getKeyAndIV = function (password, salt) {

    var ivBitLength = 128;
    var keyBitLength = 256;

    var ivByteLength = ivBitLength / 8;
    var keyByteLength = keyBitLength / 8;

    var ivWordLength = ivBitLength / 32;
    var keyWordLength = keyBitLength / 32;

    var totalWordLength = ivWordLength + keyWordLength;

    var iterations = 1000;
    var allBits = CryptoJS.PBKDF2(password, salt, { keySize: totalWordLength, iterations: iterations });

    var iv128Bits = wordArraySubArray(allBits, 0, ivByteLength);
    var key256Bits = wordArraySubArray(allBits, ivByteLength, keyByteLength);

    return {
        iv: iv128Bits,
        key: key256Bits
    };
};

var skey = getKeyAndIV("Password01", salt);

var data = CryptoJS.AES.encrypt(message, skey.key, { iv: skey.iv, padding: CryptoJS.pad.Pkcs7 }); // , format: JsonFormatter

$(".output_text").val(data.ciphertext.toString(CryptoJS.enc.Base64));
$(".output_key").val(data.key.toString(CryptoJS.enc.Base64));
$(".output_iv").val(data.iv.toString(CryptoJS.enc.Base64));


var ciphertext = CryptoJS.enc.Base64.parse($(".output_text").val());
var key = CryptoJS.enc.Base64.parse($(".output_key").val());
var iv = CryptoJS.enc.Base64.parse($(".output_iv").val());

var params = {
  ciphertext: ciphertext,
  salt: ""
};

var clearText = CryptoJS.AES.decrypt(params, key, { iv: iv, padding: CryptoJS.pad.Pkcs7 });

$(".output2").val(clearText.toString(CryptoJS.enc.Utf8));

var dataToSend = {
  password: "Password01",
  salt: salt.toString(CryptoJS.enc.Base64),
  ciphertext: data.ciphertext.toString(CryptoJS.enc.Base64),

  key: data.key.toString(CryptoJS.enc.Base64),
  iv: data.iv.toString(CryptoJS.enc.Base64)
}

var jsonData = JSON.stringify(dataToSend);

$.ajax("/api/service/decrypt", {
  type: 'post',
  data: jsonData,
  contentType: 'application/json',
  dataType: 'json'
}).done(function(x) {
  alert(x);
}).fail(function(x) {
  alert(x.responseText);
});

