function decodeUplink(input) {
  var bytes = input.bytes;
  var data = {};

  // Air - Température (2 octets, Facteur 10) - Index 0-1
  data.temperature = (bytes[0] << 8 | bytes[1]) / 10;

  // Air - Pression atm. (2 octets, Facteur 1) - Index 2-3
  data.pression = (bytes[2] << 8 | bytes[3]);

  // Air - Humidité (1 octet, Facteur 1) - Index 4
  data.humidite = bytes[4];

  // Air - SO2 (4 octets, Facteur 100) - Index 5-8
  data.so2 = (bytes[5] << 24 | bytes[6] << 16 | bytes[7] << 8 | bytes[8]) / 100;

  // Air - NOx (4 octets, Facteur 100) - Index 9-12
  data.nox = (bytes[9] << 24 | bytes[10] << 16 | bytes[11] << 8 | bytes[12]) / 100;

  // Air - NO2 (4 octets, Facteur 100) - Index 13-16
  data.no2 = (bytes[13] << 24 | bytes[14] << 16 | bytes[15] << 8 | bytes[16]) / 100;

  // Air - Ozone (O3) (4 octets, Facteur 100) - Index 17-20
  data.ozone = (bytes[17] << 24 | bytes[18] << 16 | bytes[19] << 8 | bytes[20]) / 100;

  // Air - PM10 (2 octets, Facteur 10) - Index 21-22
  data.pm10 = (bytes[21] << 8 | bytes[22]) / 10;

  // Air - PM2,5 (2 octets, Facteur 10) - Index 23-24
  data.pm25 = (bytes[23] << 8 | bytes[24]) / 10;

  // Air - CO2 (2 octets, Facteur 10) - Index 25-26
  data.co2 = (bytes[25] << 8 | bytes[26]) / 10;
  
  // Emplacement capteur (1 octet) - Index 27
  data.emplacement = bytes[27]; 

  return {
    data: data,
    warnings: [],
    errors: []
  };
}
