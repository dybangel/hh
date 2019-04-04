/******************************************************************************
*玩法模型列表
*******************************************************************************/
var PlayerModel={
	"SSC001":{name:"万位|千位|百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——五星直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC002":{name:"单选",length:5,playername:"时时彩——五星直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如时时彩五星直选01234 45869等'},
	"SSC003":{name:"万位|千位|百位|十位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前四直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC004":{name:"单选",length:4,playername:"时时彩——前四直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如时时彩前四直选0123 4589等'},
	"SSC005":{name:"千位|百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后四直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC006":{name:"单选",length:4,playername:"时时彩——后四直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如时时彩五星直选0123 5869等'},
	"SSC007":{name:"万位|千位|百位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC008":{name:"单选",length:3,playername:"时时彩——前三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如123,895等'},
	"SSC009":{name:"千位|百位|十位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——中三直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC010":{name:"单选",length:3,playername:"时时彩——中三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如123,895等'},
	"SSC011":{name:"百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后三直选复式",fun:"direct",tips:'每一位至少选择一个号码进行投注,开奖号码与所选号码相同且位置一致即为中奖'},
	"SSC012":{name:"单选",length:3,playername:"时时彩——后三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如123,895等'},
	"SSC013":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选三",fun:"direct",tips:'至少选择两个号码投注，竞猜开奖号码前三位，开奖号码为组三形态，且号码都选中即中奖 '},
	"SSC014":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选六",fun:"direct",tips:'至少选择三个号码投注，竞猜开奖号码前三位，开奖号码为组六形态，且号码都选中即中奖 '},
	"SSC015":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选三",fun:"direct",tips:'至少选择两个号码投注，竞猜开奖号码中三位，开奖号码为组三形态，且号码都选中即中奖 '},
	"SSC016":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选六",fun:"direct",tips:'至少选择三个号码投注，竞猜开奖号码中三位，开奖号码为组六形态，且号码都选中即中奖 '},
	"SSC017":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选三",fun:"direct",tips:'至少选择两个号码投注，竞猜开奖号码后三位，开奖号码为组三形态，且号码都选中即中奖 '},
	"SSC018":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三组选六",fun:"direct",tips:'至少选择三个号码投注，竞猜开奖号码后三位，开奖号码为组六形态，且号码都选中即中奖 '},
	"SSC019":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27",playername:"时时彩——前三和值",fun:"direct",tips:'至少选择一个号码，与开奖前三位号码之和相同，即为中奖'},
	"SSC020":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27",playername:"时时彩——中三和值",fun:"direct",tips:'至少选择一个号码，与开奖中三位号码之和相同，即为中奖'},
	"SSC021":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25,26,27",playername:"时时彩——后三和值",fun:"direct",tips:'至少选择一个号码，与开奖后三位号码之和相同，即为中奖'},
	"SSC022":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三和尾",fun:"direct",tips:'至少选择一个号码，与开奖前三位号码之和尾数相同，即为中奖'},
	"SSC023":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——中三和尾",fun:"direct",tips:'至少选择一个号码，与开奖中三位号码之和尾数相同，即为中奖'},
	"SSC024":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后三和尾",fun:"direct",tips:'至少选择一个号码，与开奖后三位号码之和尾数相同，即为中奖'},
	"SSC025":{name:"万位|千位|百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——任选三星复式",fun:"direct",tips:'万千百十个任选三个号码所选号码位数与开奖号码对应则中奖'},
	"SSC026":{name:"单选",length:3,playername:"时时彩——任选三星单式",fun:"SSC026",tips:'先定位选号位数，然后填写单式号码,选号位数号码与所开号码位数相同即为中奖'},
	"SSC027":{name:"万位|千位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前二直选复式",fun:"direct",tips:'每一位至少选择一个号码,与开奖号码相同切位置一致则中奖'},
	"SSC028":{name:"十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后二直选复式",fun:"direct",tips:'每一位至少选择一个号码,与开奖号码相同切位置一致则中奖'},
	"SSC029":{name:"单选",length:2,playername:"时时彩——前二直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如时时彩前二直选01 23 78等'},
	"SSC030":{name:"单选",length:2,playername:"时时彩——后二直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如时时彩后二直选01 23 78等'},
	"SSC031":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前二组选二",fun:"direct",tips:'至少选择两个号码'},
	"SSC032":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后二组选二",fun:"direct",tips:'至少选择两个号码'},
	"SSC033":{name:"万位|千位|百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——任选二星复式",fun:"direct",tips:'任两位选择任意号码,与开奖号码相同且位置一致即为中奖'},
	"SSC034":{name:"单选",length:2,playername:"时时彩——任选二星单式",fun:"SSC034",tips:'先定位选号位数，然后填写单式号码,选号位数号码与所开号码位数相同即为中奖'},
	"SSC036":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18",playername:"时时彩——前二和值",fun:"direct",tips:'至少选择一个号码，与开奖前二位号码之和相同，即为中奖'},
	"SSC037":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18",playername:"时时彩——后二和值",fun:"direct",tips:'至少选择一个号码，与开奖后二位号码之和相同，即为中奖'},
	"SSC038":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前二和尾",fun:"direct",tips:'至少选择一个号码，与开奖前二位号码之和尾数相同，即为中奖'},
	"SSC039":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后二和尾",fun:"direct",tips:'至少选择一个号码，与开奖后二位号码之和尾数相同，即为中奖'},
	"SSC040":{name:"万位|千位|百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——定位胆",fun:"direct",tips:'任意选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"SSC041":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——五星不定位",fun:"direct",tips:'任选一个号码为一注，当选择号码与开奖结果任意一位数相同即为中奖'},
	"SSC042":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——前三不定位",fun:"direct",tips:'任选一个号码为一注，当选择号码与开奖结果前三位任意一位数相同即为中奖'},
	"SSC043":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"时时彩——后三不定位",fun:"direct",tips:'任选一个号码为一注，当选择号码与开奖结果后三位任意一位数相同即为中奖'},
	"SSC044":{name:"十位|个位",code:"大,小,单,双",playername:"时时彩——大小单双",fun:"SSC044",tips:'每位任意选择一个号码,所选组号与开奖结果符合即为中奖'},
	"PK10001":{name:"第一位|第二位|第三位|第四位",code:"01,02,03,04,05,06,07,08,09,10",playername:"PK10前四直选复式",fun:"direct",tips:'每位任意选择一个号码,所选号码与开奖结果前四位相同且位置一致即为中奖'},
	"PK10002":{name:"单选",length:8,playername:"PK10前四直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如01020304 05080699等'},
	"PK10003":{name:"第一位|第二位|第三位",code:"01,02,03,04,05,06,07,08,09,10",playername:"PK10前三直选复式",fun:"direct",tips:'每位任意选择一个号码,所选号码与开奖结果前三位相同且位置一致即为中奖'},
	"PK10004":{name:"单选",length:6,playername:"PK10前三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如020304 080699等'},
	"PK10005":{name:"第一位|第二位",code:"01,02,03,04,05,06,07,08,09,10",playername:"PK10前二直选复式",fun:"direct",tips:'每位任意选择一个号码,所选号码与开奖结果前三位相同且位置一致即为中奖'},
	"PK10006":{name:"单选",length:4,playername:"PK10前二直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如0304 0899等'},
	"PK10007":{name:"第一位|第二位|第三位|第四位|第五位|第六位|第七位|第八位|第九位|第十位",code:"01,02,03,04,05,06,07,08,09,10",playername:"PK10定位胆",fun:"direct",tips:'任意选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"SYXW001":{name:"第一位|第二位|第三位",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五前三直选复式",fun:"direct",tips:'从每位各选1个或多个号码，所选号码与开奖号码前三位号码相同且顺序一致即为中奖'},
	"SYXW002":{name:"单选",length:6,playername:"十一选五前三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	"SYXW003":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-前三组选复式",fun:"direct",tips:'任选3个或多个号码，所选号码与开奖号码前三位号码相同顺序不限即为中奖'},
	"SYXW004":{name:"单选",length:6,playername:"十一选五-前三组选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	
	"SYXW005":{name:"第二位|第三位|第四位",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-中三直选复式",fun:"direct",tips:'从每位各选1个或多个号码，所选号码与开奖号码中间三位号码相同且顺序一致即为中奖'},
	"SYXW006":{name:"单选",length:6,playername:"十一选五-中三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	"SYXW007":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-中三组选复式",fun:"direct",tips:'任选3个或多个号码，所选号码与开奖号码中间三位号码相同顺序不限即为中奖'},
	"SYXW008":{name:"单选",length:6,playername:"十一选五-中三组选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	
	"SYXW009":{name:"第三位|第四位|第五位",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-后三直选复式",fun:"direct",tips:'从每位各选1个或多个号码，所选号码与开奖号码后三位号码相同且顺序一致即为中奖'},
	"SYXW010":{name:"单选",length:6,playername:"十一选五-后三直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	"SYXW011":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-后三组选复式",fun:"direct",tips:'任选3个或多个号码，所选号码与开奖号码后三位号码相同顺序不限即为中奖'},
	"SYXW012":{name:"单选",length:6,playername:"十一选五-后三组选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如030405 060809等'},
	
	"SYXW013":{name:"第一位|第二位",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-前二直选复式",fun:"direct",tips:'从每位各选1个或多个号码，所选号码与开奖号码前二位号码相同且顺序一致即为中奖'},
	"SYXW014":{name:"单选",length:4,playername:"十一选五-前二直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如0203 0608等'},
	"SYXW015":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-前二组选复式",fun:"direct",tips:'任选2个或多个号码，所选号码与开奖号码前二位号码相同顺序不限即为中奖'},
	"SYXW016":{name:"单选",length:4,playername:"十一选五-前二组选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如0304 0609等'},
	
	"SYXW017":{name:"第四位|第五位",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-后二直选复式",fun:"direct",tips:'从每位各选1个或多个号码，所选号码与开奖号码后二位号码相同且顺序一致即为中奖'},
	"SYXW018":{name:"单选",length:4,playername:"十一选五-后二直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如0203 0608等'},
	"SYXW019":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-后二组选复式",fun:"direct",tips:'任选2个或多个号码，所选号码与开奖号码后二位号码相同顺序不限即为中奖'},
	"SYXW020":{name:"单选",length:4,playername:"十一选五-后二组选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.选号格式如0304 0609等'},
	"SYXW021":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选一",fun:"direct",tips:'任选1个或多个号码，所选号码与开奖号码任意1个号码相同即为中奖'},
	"SYXW022":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选二",fun:"direct",tips:'任选2个或多个号码，所选号码与开奖号码任意2个号码相同即为中奖'},
	"SYXW023":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选三",fun:"direct",tips:'任选3个或多个号码，所选号码与开奖号码任意3个号码相同即为中奖'},
	"SYXW024":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选四",fun:"direct",tips:'任选4个或多个号码，所选号码与开奖号码任意4个号码相同即为中奖'},
	"SYXW025":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选五",fun:"direct",tips:'任选5个或多个号码，所选号码与开奖号码相同即为中奖'},
	"SYXW026":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选六",fun:"direct",tips:'任选6个或多个号码，所选号码与开奖号码五个号码相同即为中奖'},
	"SYXW027":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选七",fun:"direct",tips:'任选7个或多个号码，所选号码与开奖号码五个号码相同即为中奖'},
	"SYXW028":{name:"选择号码",code:"01,02,03,04,05,06,07,08,09,10,11",playername:"十一选五-任选八",fun:"direct",tips:'任选8个或多个号码，所选号码与开奖号码五个号码相同即为中奖'},
	
	"KS001":{name:"选择号码",code:"3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18",playername:"快三-和值",fun:"direct",tips:'任选一个号码为一注,与开奖号码总和数相同即为中奖'},
	"KS002":{name:"选择号码",code:"三同号通选",playername:"快三-三同号通选",fun:"kuaisan",tips:'三个开奖号码一致(豹子)即为中奖'},
	"KS003":{name:"选择号码",code:"111,222,333,444,555,666",playername:"快三-三同号单选",fun:"kuaisan",tips:'选择号码与开奖号码相同即为中奖'},
	"KS004":{name:"选择号码",code:"1,2,3,4,5,6",playername:"快三-三不同号",fun:"direct",tips:'至少选择3个不同号码投注，所选号码与开奖号码一致即中奖'},
	"KS005":{name:"选择号码",code:"三连号通选",playername:"快三-三连号通选",fun:"kuaisan",tips:'对所有3个相连的号码（123、234、345、456）进行投注，任意号码开出即中奖'},
	"KS006":{name:"选择号码",code:"11,22,33,44,55,66",playername:"快三-二同号复选",fun:"kuaisan",tips:'开奖号码的任意2位，与您投注的二同号一致即中奖'},
	"KS007":{name:"选择号码",code:"11,22,33,44,55,66",playername:"快三-二同号单选",fun:"KS007",tips:'开奖号码的任意2位，与您投注的二同号一致即中奖'},
	"KS008":{name:"选择号码",code:"1,2,3,4,5,6",playername:"快三-二不同号",fun:"direct",tips:'至少选择2个不同号码投注，所选号码与开奖号码一致即中奖'},
	
	"FC001":{name:"百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-三星直选复式",fun:"direct",tips:'每一位至少选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"FC002":{name:"单选",length:3,playername:"福彩-三星直选单式",fun:"unitary",tips:'请将单式投注号码粘贴到文本框之中,每注选号都用逗号(英文状态)分开.如012 784等'},
	"FC003":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-三星组选组六",fun:"direct",tips:'任意选择3个号码组成一注，所选号码与开奖号码相同，且顺序不限，即为中奖。'},
	"FC004":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-三星组选组三",fun:"direct",tips:'任意选择2个号码组成两注，所选号码与开奖号码相同，且顺序不限，即为中奖。'},
	"FC005":{name:"单选",length:3,playername:"福彩-三星组选混合",fun:"unitary",tips:'手动输入一个3位数号码组成一注(不包含豹子号)，开奖号码后3位为组选三或组选六形态，投注号码与开奖号码后三位相同，顺序不限，即为中奖。'},
	"FC006":{name:"十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-后二直选复式",fun:"direct",tips:'每一位至少选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"FC007":{name:"单选",length:2,playername:"福彩-后二直选单式",fun:"unitary",tips:'手动输入一个2个号码组成一注，开奖号码后2位与所选号码相同即为中奖。'},
	"FC008":{name:"百位|十位",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-前二直选复式",fun:"direct",tips:'每一位至少选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"FC009":{name:"单选",length:2,playername:"福彩-前二直选单式",fun:"unitary",tips:'手动输入一个2个号码组成一注，开奖号码前2位与所选号码相同即为中奖。'},
	"FC010":{name:"百位|十位|个位",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-定位胆",fun:"direct",tips:'至少选择一个号码,与开奖号码相同且位数一致即为中奖'},
	"FC011":{name:"选择号码",code:"0,1,2,3,4,5,6,7,8,9",playername:"福彩-不定位胆",fun:"direct",tips:'至少选择一个号码,在开奖号码中出现即为中奖'},
};
/******************************************************************************************************
*加载玩法数据信息
*******************************************************************************************************/
var playerLoading=function(options)
{
	var options = options || {};
	try{
		if(options!=undefined && typeof(options)=='object' 
		&& PlayerModel!=undefined && typeof(PlayerModel)=='object'
		&& options["playermode"]!=undefined && options["playermode"]!="")
		{
			var jsnMode = PlayerModel[options["playermode"]];
			if(jsnMode!=undefined && typeof(jsnMode)=='object')
			{
				try{
					if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="direct"){
						ChooseDirect(jsnMode);	
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="unitary"){
						ChooseUnitary(jsnMode);	
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="SSC044")
					{
						ChooseSSC044(jsnMode);
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="SSC034")
					{
						ChooseSSC034(jsnMode);
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="SSC026")
					{
						ChooseSSC026(jsnMode);
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="kuaisan")
					{
						ChooseKuaisan(jsnMode);
					}
					else if(jsnMode["fun"]!=undefined && jsnMode["fun"]=="KS007")
					{
						ChooseKS007(jsnMode);
					}
				}catch(err){}
				/***********************************************************************
				*加载玩法奖金信息
				************************************************************************/
				try{bonusLoading(options);}catch(err){}
				/***********************************************************************
				*将当前选择的玩法设置为默认信息
				************************************************************************/
				try{
					if(options["playerid"]!=undefined && options["playerid"]!="" && !isNaN(options["playerid"])
					&& options["classid"]!=undefined && options["classid"]!="" && !isNaN(options["classid"]))
					{
						var lotteryKey = cfg["lotterykey"] || "demo";
						jQuery.cookies("fooke_"+lotteryKey+"_player",options["playermode"]);
						jQuery.cookies("fooke_"+lotteryKey+"_class",options["classid"]);
					}
				}catch(err){}
				/***********************************************************************
				*加载选号按钮点击事件
				************************************************************************/
				$("#frmControl").find("td[operate=\"code\"]").find("a").click(function(){
					try{
						var selected = $(this).attr("choose") || "false";
						if(selected==undefined || selected=="false" || selected==""){
							selected="false";
						}
						if(selected=="true"){$(this).attr("choose","false");}
						else{$(this).attr("choose","true");}
						try{
							var thisTD = $(this.parentNode.parentNode).find("td[operate=\"code\"]");
							try{processCode(thisTD,options);}catch(err){}
						}catch(err){}
						
					}catch(err){}
				});
				$("#frmControl").find("#frm-unitaryCode").change(function(){
					try{
						var strValue = this.value;
						try{
							strValue = jQuery.trim(strValue);
							strValue=strValue.replace(/[a-zA-Z]/g,",");
							strValue=strValue.replace(/r\/\n/g,",");
							strValue=strValue.replace(/\n/g,",");
							strValue = strValue.replace(/\D/,",");
							strValue = strValue.replace(new RegExp('	',"gm"),',');
							strValue = strValue.replace(new RegExp('，',"gm"),',');
							strValue = strValue.replace(new RegExp(' ',"gm"),',');
							strValue = strValue.replace(new RegExp('　',"gm"),',');
							strValue = strValue.replace(new RegExp('( +)',"gm"),',');
							strValue = jQuery.trim(strValue);
							this.value = strValue;
						}catch(err){}
						
						try{
							var thisTD = $(this.parentNode.parentNode).find("td[operate=\"code\"]");
							$(thisTD).attr("text",strValue);
							try{processCode(thisTD,options);}catch(err){}
						}catch(err){}
						
					}catch(err){}
				});
				/***********************************************************************
				*加载快速选号点击事件
				************************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"all\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						$(thisTD).find("a").each(function(){
							$(this).attr("choose","true");								  
						});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
				/*******************************************************************
				*快速选择单号
				********************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"single\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						$(thisTD).find("a").each(function(){
							var number = parseInt($(this).attr("number"));
							if(number!=undefined && !isNaN(number) && number % 2==1){
								$(this).attr("choose","true");	
							};
						});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
				/*******************************************************************
				*快速选择双号
				********************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"double\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						$(thisTD).find("a").each(function(){
							var number = parseInt($(this).attr("number"));
							if(number!=undefined && !isNaN(number) && number % 2==0){
								$(this).attr("choose","true");	
							}							  
						});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
				/*******************************************************************
				*快速选择大号
				********************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"will\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						var Length = $(thisTD).find("a").length;
						$(thisTD).find("a").each(function(){
							var number = parseInt($(this).attr("number"));
							if(number!=undefined && !isNaN(number) && number >=(Length/2)){
								$(this).attr("choose","true");	
							}
						});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
				
				/*******************************************************************
				*快速选择大号
				********************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"small\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						var Length = $(thisTD).find("a").length;
						$(thisTD).find("a").each(function(){
							var number = parseInt($(this).attr("number"));
							if(number!=undefined && !isNaN(number) && number <(Length/2)){
								$(this).attr("choose","true");	
							}
						});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
				
				/*******************************************************************
				*快速选择大号
				********************************************************************/
				$("#frmControl").find("td[operate=\"quickselection\"]").find("a[operate=\"clear\"]").click(
				function(){
					try{
						var Kid = parseInt($(this).parents("tr[class=\"hmenu\"]").attr("Kid")) || 0;
						var thisTD = $("#frm-code").find("tr[kvalue=\""+Kid+"\"]").find("td[operate=\"code\"]");
						$(thisTD).find("a").each(function(){$(this).attr("choose","false");});
						try{processCode(thisTD,options);}catch(err){}
					}catch(err){}
				});
			}
			else{
				alert('玩法列表加载错误,请重试！',function(){
					window.location.reload();
				});	
				return false;
			}
		}
	}catch(err){}	
};
/***********************************************************************************************************
*将选中的号码全部清空
************************************************************************************************************/
var EmptyChooseCode = function(){
	/****************************************************************************
	*删除单式的号码
	*****************************************************************************/
	try{
		if(document.getElementById('frm-unitaryCode')){
			document.getElementById('frm-unitaryCode').value="";	
		}	
	}catch(err){}
	/****************************************************************************
	*删除复式的选号信息
	*****************************************************************************/
	try{$("#frm-code").find("td[operate=\"code\"]").find("a").attr("choose","false");}
	catch(err){}
	/****************************************************************************
	*清除号码中的提示内容
	*****************************************************************************/
	try{$("#frmLotteryAmounts").html('当前未选择任何号码');}catch(err){}
}

/***********************************************************************************************************
*设置选中的号码信息
************************************************************************************************************/
var processCode = function(thisTD,player)
{
	var chooseLength=0;
	var chooseText="-1";
	/*******************************************************************
	*设置选中的数据信息
	********************************************************************/
	if(document.getElementById('frm-unitaryCode')!=undefined 
	&& document.getElementById('frm-unitaryCode')!=null)
	{
		try{
			chooseText = $("#frm-unitaryCode").val();
			chooseLength = chooseText.split(',').length;
			try{
				
				if(player!=undefined && typeof(player)=='object' 
				&& PlayerModel!=undefined && typeof(PlayerModel)=='object'
				&& player["playermode"]!=undefined && player["playermode"]!="")
				{
					try{
						var jsnModel = PlayerModel[player["playermode"]];
						if(jsnModel!=undefined && typeof(jsnModel)=='object' 
						&& jsnModel['length']!=undefined && jsnModel['length']!=''){
							var arrTemp = chooseText.split(',');
							var sLength = parseInt(jsnModel['length']) || 0;
							var charLength = (sLength * arrTemp.length+arrTemp.length-1);
							if(charLength!=chooseText.length){
								alert('单式选号格式错误！');return false;
							}
						}
					}catch(err){}
				}
			}catch(err){}
		}catch(err){}
	}else{
		try{
			$(thisTD).find("a[choose=\"true\"]").each(function(){
				chooseLength=chooseLength+1;
				var disText = $(this).attr("number") || "";
				if(disText!=undefined && disText!=""){
					chooseText = chooseText =="-1" ? disText : chooseText + "," + disText;		
				}
			});
		}catch(err){};	
	}
	/*******************************************************************
	*将特殊字符替换掉
	********************************************************************/
	try{
		chooseText = chooseText.replace(' ','');
		chooseText = chooseText.replace('  ','');
		chooseText = chooseText.replace('  ','');
	}catch(err){}
	/*******************************************************************
	*将数据附加到指定的数值中
	********************************************************************/
	try{	
		$(thisTD).attr("text",chooseText);
		$(thisTD).attr("length",chooseLength);
	}catch(err){};
	/*******************************************************************
	*计算选号信息
	********************************************************************/
	
	try{
		var codeText = "";
		$("#frm-code").find("td[operate=\"code\"]").each(function(){
			var text = $(this).attr("text") || "-1";
			if(text!=undefined && text!=""){
				if(codeText!=""){codeText=codeText+"|"+text;}
				else{codeText=text;}
			}
		});
		try{$("#frm-code").attr("code",codeText);}catch(err){}
	}catch(err){}
	/*******************************************************************
	*计算选择注数信息
	********************************************************************/
	try{
		if(player!=undefined && typeof(player)=='object' 
		&& player["playermode"]!=undefined && player["playermode"]!="")
		{
			calculationLoading(player,function(number,chooseText){
				if(number!=0 && chooseText!="" && chooseText!="-1"){
					try{
						$("#frm-number").html(number);
						var Multiple = parseFloat($("#frmMultiple").val()) || 1;
						var AnitAmount = parseFloat(cfg["unitamount"]) || 2;
						var thisAmount = parseFloat(((AnitAmount * number) * Multiple)).toFixed(2);
						$("#frm-amount").html(thisAmount);
						$("#frmLotteryAmounts").html("共选择了"+number+"注,合计"+thisAmount+"元");
					}catch(err){}
				}else{
					try{
						$("#frm-number").html(0);
						$("#frm-amount").html(0);
						$("#frmLotteryAmounts").html("当前未选择任何号码");		
					}catch(err){}
				};
			});
		}		
	}catch(err){}
}

var calculationLoading = function(options,back){
	var options = options || {};
	var number = 0;
	var chooseText = $("#frm-code").attr("code");
	try{
		if(options!=undefined && typeof(options)=='object' 
		&& options["playermode"]!=undefined && options["playermode"]!=""
		&& chooseText!="" && chooseText!=undefined)
		{
			switch(options["playermode"]){
				case "SSC001":number = SSC001(chooseText);break;
				case "SSC002":number = SSC002(chooseText);break;
				case "SSC003":number = SSC003(chooseText);break;
				case "SSC004":number = SSC004(chooseText);break;
				case "SSC005":number = SSC005(chooseText);break;
				case "SSC006":number = SSC006(chooseText);break;
				case "SSC007":number = SSC007(chooseText);break;
				case "SSC008":number = SSC008(chooseText);break;
				case "SSC009":number = SSC009(chooseText);break;
				case "SSC010":number = SSC010(chooseText);break;
				case "SSC011":number = SSC011(chooseText);break;
				case "SSC012":number = SSC012(chooseText);break;
				case "SSC013":number = SSC013(chooseText);break;
				case "SSC014":number = SSC014(chooseText);break;
				case "SSC015":number = SSC015(chooseText);break;
				case "SSC016":number = SSC016(chooseText);break;
				case "SSC017":number = SSC017(chooseText);break;
				case "SSC018":number = SSC018(chooseText);break;
				case "SSC019":number = SSC019(chooseText);break;
				case "SSC020":number = SSC020(chooseText);break;
				case "SSC021":number = SSC021(chooseText);break;
				case "SSC022":number = SSC022(chooseText);break;
				case "SSC023":number = SSC023(chooseText);break;
				case "SSC024":number = SSC024(chooseText);break;
				case "SSC025":number = SSC025(chooseText);break;
				case "SSC026":number = SSC026(chooseText,function(newCode){chooseText=newCode;});break;
				case "SSC027":number = SSC027(chooseText);break;
				case "SSC028":number = SSC028(chooseText);break;
				case "SSC029":number = SSC029(chooseText);break;
				case "SSC030":number = SSC030(chooseText);break;
				case "SSC031":number = SSC031(chooseText);break;
				case "SSC032":number = SSC032(chooseText);break;
				case "SSC033":number = SSC033(chooseText);break;
				case "SSC034":number = SSC034(chooseText,function(newCode){chooseText=newCode;});break;
				case "SSC035":number = SSC035(chooseText);break;
				case "SSC036":number = SSC036(chooseText);break;
				case "SSC037":number = SSC037(chooseText);break;
				case "SSC038":number = SSC038(chooseText);break;
				case "SSC039":number = SSC039(chooseText);break;
				case "SSC040":number = SSC040(chooseText);break;
				case "SSC041":number = SSC041(chooseText);break;
				case "SSC042":number = SSC042(chooseText);break;
				case "SSC043":number = SSC043(chooseText);break;
				case "SSC044":number = SSC044(chooseText);break;
				
				case "PK10001":number = PK10001(chooseText);break;
				case "PK10002":number = PK10002(chooseText);break;
				case "PK10003":number = PK10003(chooseText);break;
				case "PK10004":number = PK10004(chooseText);break;
				case "PK10005":number = PK10005(chooseText);break;
				case "PK10006":number = PK10006(chooseText);break;
				case "PK10007":number = PK10007(chooseText);break;
				
				case "SYXW001":number = SYXW001(chooseText);break;
				case "SYXW002":number = SYXW002(chooseText);break;
				case "SYXW003":number = SYXW003(chooseText);break;
				case "SYXW004":number = SYXW004(chooseText);break;
				case "SYXW005":number = SYXW005(chooseText);break;
				case "SYXW006":number = SYXW006(chooseText);break;
				case "SYXW007":number = SYXW007(chooseText);break;
				case "SYXW008":number = SYXW008(chooseText);break;
				case "SYXW009":number = SYXW009(chooseText);break;
				case "SYXW010":number = SYXW010(chooseText);break;
				case "SYXW011":number = SYXW011(chooseText);break;
				case "SYXW012":number = SYXW012(chooseText);break;
				case "SYXW013":number = SYXW013(chooseText);break;
				case "SYXW014":number = SYXW014(chooseText);break;
				case "SYXW015":number = SYXW015(chooseText);break;
				case "SYXW016":number = SYXW016(chooseText);break;
				case "SYXW017":number = SYXW017(chooseText);break;
				case "SYXW018":number = SYXW018(chooseText);break;
				case "SYXW019":number = SYXW019(chooseText);break;
				case "SYXW020":number = SYXW020(chooseText);break;
				case "SYXW021":number = SYXW021(chooseText);break;
				case "SYXW022":number = SYXW022(chooseText);break;
				case "SYXW023":number = SYXW023(chooseText);break;
				case "SYXW024":number = SYXW024(chooseText);break;
				case "SYXW025":number = SYXW025(chooseText);break;
				case "SYXW026":number = SYXW026(chooseText);break;
				case "SYXW027":number = SYXW027(chooseText);break;
				case "SYXW028":number = SYXW028(chooseText);break;
				
				case "KS001":number = KS001(chooseText);break;
				case "KS002":number = KS002(chooseText);break;
				case "KS003":number = KS003(chooseText);break;
				case "KS004":number = KS004(chooseText);break;
				case "KS005":number = KS005(chooseText);break;
				case "KS006":number = KS006(chooseText);break;
				case "KS007":number = KS007(chooseText);break;
				case "KS008":number = KS008(chooseText);break;
				
				case "FC001":number = FC001(chooseText);break;
				case "FC002":number = FC002(chooseText);break;
				case "FC003":number = FC003(chooseText);break;
				case "FC004":number = FC004(chooseText);break;
				case "FC005":number = FC005(chooseText);break;
				case "FC006":number = FC006(chooseText);break;
				case "FC007":number = FC007(chooseText);break;
				case "FC008":number = FC008(chooseText);break;
				case "FC009":number = FC009(chooseText);break;
				case "FC010":number = FC010(chooseText);break;
				case "FC011":number = FC011(chooseText);break;
				
			};
		}	
	}catch(err){
		alert('玩法注数计算过程中发生未知错误,请重试！'+err.message);
		return false;
	};
	/****************************************************************************
	*返回数据信息
	*****************************************************************************/
	if(chooseText!=undefined && chooseText!="" && chooseText!="-1"){
		try{
			if(back!=undefined && typeof(back)=='function')
			{back(number,chooseText);}
		}catch(err){}
	};
};

/*******************************************************************************
*福彩-不定位
********************************************************************************/
var FC011=function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	for(var a=0;a<arrayTemp.length;a++){
		if(!parseInt(arrayTemp[a])){return 0;}
		else if(parseInt(arrayTemp[a])<0){return 0;}
		else if(parseInt(arrayTemp[a])>=10){return 0;}
	}
	return arrayTemp.length;
};

/*******************************************************************************
*福彩-定位胆
********************************************************************************/
var FC010=function(chooseText)
{
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=3){return 0;}
	var number=0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].indexOf("-1")!=-1){continue;}
		else{number=number+arrayTemp[a].split(",").length;}
	}
	return number;
};

/*******************************************************************************
*福彩-前二直选单式
********************************************************************************/
var FC009=function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=2){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*福彩-前二组选复式
********************************************************************************/
var FC008=function(chooseText)
{
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var number=1;
	for(var a=0;a<arrayTemp.length;a++){
		number=number*arrayTemp[a].split(",").length;
	}
	return number;	
};

/*******************************************************************************
*福彩-后二直选单式
********************************************************************************/
var FC007=function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=2){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*福彩-后二组选复式
********************************************************************************/
var FC006=function(chooseText)
{
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var number=1;
	for(var a=0;a<arrayTemp.length;a++){
		number=number*arrayTemp[a].split(",").length;
	}
	return number;	
};

/*******************************************************************************
*福彩-三星组选混合
********************************************************************************/
var FC005=function(chooseText)
{
	var arrayTemp = selectCode.split(",");
	var isTrue = true;
	for(var a=0;a<arrayTemp.length;a++){
		var listing	= arrayTemp[a];
		if(listing.length!=3){isTrue=false;break;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return arrayTemp.length;
};


/*******************************************************************************
*福彩-三星组三组三
********************************************************************************/
var FC004=function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var arrayTemp = chooseText.split(",");
		var length = arrayTemp.length;
		if(length<=1){return 0;}
		else if(length>=11){return 0;}
		else{return length*(length-1);}
	}catch(err){}
};

/*******************************************************************************
*福彩-三星组三组六
********************************************************************************/
var FC003=function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split(",");
	var number=0;
	if(arrayTemp.length<=2){return 0;}
	for(var n=0;n<arrayTemp.length;n++){
	  for(var j=n+1;j<arrayTemp.length;j++){
		  for(var k=j+1;k<arrayTemp.length;k++){
			 number=number+1;
		  };	
	  };
	};
	return number;
};

/*******************************************************************************
*福彩-三星直选复式
********************************************************************************/
var FC002=function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=3){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*福彩-三星直选复式
********************************************************************************/
var FC001=function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==3){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
};

/*******************************************************************************
*快三-二同号单选
********************************************************************************/
var KS008=function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split(",");
	var number=0;
	if(arrayTemp.length<2){return 0;}
	for(var n=0;n<arrayTemp.length;n++){
		for(var j=n+1;j<arrayTemp.length;j++){
		  number=number+1;
		}	
	}
	return number;
};
/*******************************************************************************
*快三-二同号单选
********************************************************************************/
var KS007=function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var number=0;
	var aTemp = arrayTemp[0].split(",");
	var bTemp = arrayTemp[1].split(",");
	for(var a=0;a<aTemp.length;a++){
		for(var b=0;b<bTemp.length;b++){
			if((parseInt(aTemp[a])/parseInt(bTemp[b]))!=11){
				number=number+1;
			}
		}
	}
	return number;
};
/*******************************************************************************
*快三-二同号复选
********************************************************************************/
var KS006=function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=0){return 0;}
	else if(arrayTemp.length>=7){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=10){return 0;}
		   else if(parseInt(arrayTemp[n])>=67){return 0;}
		}
	}catch(err){}
    return arrayTemp.length;
};
/*******************************************************************************
*快三-三连号通选
********************************************************************************/
var KS005=function(chooseText){
	if (chooseText!="三连号通选") { return 0; }
	else{ return 1;}
};
/*******************************************************************************
*快三-三不同号
********************************************************************************/
var KS004=function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split(",");
	var number=0;
	if(arrayTemp.length<=2){return 0;}
	for(var n=0;n<arrayTemp.length;n++){
	  for(var j=n+1;j<arrayTemp.length;j++){
		  for(var k=j+1;k<arrayTemp.length;k++){
			 number=number+1;
		  }	
	  }	
	}
	return number;
};
/*******************************************************************************
*快三-三同号单选
********************************************************************************/
var KS003=function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=0){return 0;}
	else if(arrayTemp.length>=7){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=100){return 0;}
		   else if(parseInt(arrayTemp[n])>=667){return 0;}
		}
	}catch(err){}
    return arrayTemp.length;
};
/*******************************************************************************
*快三-三同号通选
********************************************************************************/
var KS002=function(chooseText){
	if (chooseText!="三同号通选") { return 0; }
	else{ return 1;}
};
/*******************************************************************************
*快三-和值
********************************************************************************/
var KS001=function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=0){return 0;}
	else if(arrayTemp.length>=16){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=2){return 0;}
		   else if(parseInt(arrayTemp[n])>=19){return 0;}
		}
	}catch(err){}
    return arrayTemp.length;
};

/*******************************************************************************
*十一选五-任选八
********************************************************************************/
var SYXW028 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=7){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
	if(arrayTemp.length==8){number=1;}
	else if(arrayTemp.length==9){number=9;}
	else if(arrayTemp.length==10){number=45;}
	else if(arrayTemp.length==11){number=165;}
    return number;
};

/*******************************************************************************
*十一选五-任选七
********************************************************************************/
var SYXW027 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=6){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
	if(arrayTemp.length==7){number=1;}
	else if(arrayTemp.length==8){number=8;}
	else if(arrayTemp.length==9){number=36;}
	else if(arrayTemp.length==10){number=120;}
	else if(arrayTemp.length==11){number=330;}
    return number;
};

/*******************************************************************************
*十一选五-任选六
********************************************************************************/
var SYXW026 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=5){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
	if(arrayTemp.length==6){number=1;}
	else if(arrayTemp.length==7){number=7;}
	else if(arrayTemp.length==8){number=28;}
	else if(arrayTemp.length==9){number=84;}
	else if(arrayTemp.length==10){number=210;}
	else if(arrayTemp.length==11){number=462;}
    return number;
};

/*******************************************************************************
*十一选五-任选五
********************************************************************************/
var SYXW025 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=4){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
	if(arrayTemp.length==5){number=1;}
	else if(arrayTemp.length==6){number=6;}
	else if(arrayTemp.length==7){number=21;}
	else if(arrayTemp.length==8){number=56;}
	else if(arrayTemp.length==9){number=126;}
	else if(arrayTemp.length==10){number=252;}
	else if(arrayTemp.length==11){number=462;}
    return number;
};

/*******************************************************************************
*十一选五-任选四
********************************************************************************/
var SYXW024 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=3){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
	if(arrayTemp.length==4){number=1;}
	else if(arrayTemp.length==5){number=5;}
	else if(arrayTemp.length==6){number=15;}
	else if(arrayTemp.length==7){number=35;}
	else if(arrayTemp.length==8){number=70;}
	else if(arrayTemp.length==9){number=126;}
	else if(arrayTemp.length==10){number=210;}
	else if(arrayTemp.length==11){number=330;}
    return number;
};

/*******************************************************************************
*十一选五-任选三
********************************************************************************/
var SYXW023 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
	if(arrayTemp.length<=2){return 0;}
	else if(arrayTemp.length>=12){return 0;}
	try{
		for (var n = 0; n < arrayTemp.length; n++) {
		   if(arrayTemp[n].length!=2){return 0;}
		   else if(!parseInt(arrayTemp[n])){return 0;}
		   else if(parseInt(arrayTemp[n])<=0){return 0;}
		   else if(parseInt(arrayTemp[n])>=12){return 0;}
		}
	}catch(err){}
    var number = 0;
    if(arrayTemp.length==3){number=1;}
	else if(arrayTemp.length==4){number=4;}
	else if(arrayTemp.length==5){number=10;}
	else if(arrayTemp.length==6){number=20;}
	else if(arrayTemp.length==7){number=35;}
	else if(arrayTemp.length==8){number=56;}
	else if(arrayTemp.length==9){number=84;}
	else if(arrayTemp.length==10){number=120;}
	else if(arrayTemp.length==11){number=165;}
    return number;
};

/*******************************************************************************
*十一选五-任选二
********************************************************************************/
var SYXW022 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 1) { return 0; }
	else if(arrayTemp.length>=12){return 0;}
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
            number = number + 1;
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-任选一
********************************************************************************/
var SYXW021 = function(chooseText)
{
	try{
		if (chooseText.indexOf("-1") != -1) { return 0; }
		var arrayTemp = chooseText.split(",");
		if(arrayTemp<=0){return 0;}
		else if(arrayTemp.length>=12){return 0;}
		var number=0;
		for(var k in arrayTemp){
			if(arrayTemp[k].length!=2){return 0;}
			else if(!parseInt(arrayTemp[k])){return 0;}
			else{number=number+1;}
		}
		return number;
	}catch(err){}
};

/*******************************************************************************
*十一选五-前二组选单式
********************************************************************************/
var SYXW020 = function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=4){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		number=number+1;
	};
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*十一选五-前二组选复式
********************************************************************************/
var SYXW019 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 2) { return 0; }
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
           number = number + 1;
        }
    }
    return number;
};


/*******************************************************************************
*十一选五-前二直选单式
********************************************************************************/
var SYXW018 = function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	for (var a = 0; a < arrayTemp.length; a++) {
		if (arrayTemp[a].length != 4) { isTrue = false; break; }
		else if(!parseInt(arrayTemp[a])){ isTrue = false; break; }
		else if(isNaN(parseInt(arrayTemp[a]))){ isTrue = false; break; }
	}
	if (isTrue == false) { alert("第一组号码不正确！"); return 0; }
	return arrayTemp.length;
};


/*******************************************************************************
*十一选五-前二直选复式
********************************************************************************/
var SYXW017 = function(chooseText)
{
    if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split("|");
    if (arrayTemp.length != 2) { return 0; }
    var number = 0;
    var temp = arrayTemp[0].split(",");
    var temp1 = arrayTemp[1].split(",");
    for (var k = 0; k < temp.length; k++) {
        for (var l = 0; l < temp1.length; l++) {
            if (temp[k] != temp1[l]) {
                number=number+1;
            }
        }
    }
    return number;	
};

/*******************************************************************************
*十一选五-前二组选单式
********************************************************************************/
var SYXW016 = function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=4){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		number=number+1;
	};
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*十一选五-前二组选复式
********************************************************************************/
var SYXW015 = function(chooseText)
{
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 2) { return 0; }
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
           number = number + 1;
        }
    }
    return number;
};


/*******************************************************************************
*十一选五-前二直选单式
********************************************************************************/
var SYXW014 = function(chooseText)
{
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	for (var a = 0; a < arrayTemp.length; a++) {
		if (arrayTemp[a].length != 4) { isTrue = false; break; }
		else if(!parseInt(arrayTemp[a])){ isTrue = false; break; }
		else if(isNaN(parseInt(arrayTemp[a]))){ isTrue = false; break; }
	}
	if (isTrue == false) { alert("第一组号码不正确！"); return 0; }
	return arrayTemp.length;
};


/*******************************************************************************
*十一选五-前二直选复式
********************************************************************************/
var SYXW013 = function(chooseText)
{
    if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split("|");
    if (arrayTemp.length != 2) { return 0; }
    var number = 0;
    var temp = arrayTemp[0].split(",");
    var temp1 = arrayTemp[1].split(",");
    for (var k = 0; k < temp.length; k++) {
        for (var l = 0; l < temp1.length; l++) {
            if (temp[k] != temp1[l]) {
                number=number+1;
            }
        }
    }
    return number;	
};


/*******************************************************************************
*十一选五-后三组选单式
********************************************************************************/
var SYXW012 = function(chooseText){
    var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=6){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		number=number+1;
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;	
};

/*******************************************************************************
*十一选五-后三组选复式
********************************************************************************/
var SYXW011 = function(chooseText){
    if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 2) { return 0; }
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
            for (var k = j + 1; k < arrayTemp.length; k++) {
                number = number + 1;
            }
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-后三直选单式
********************************************************************************/
var SYXW010 = function(chooseText){
    var arrayTemp = chooseText.split(",");
    var isTrue = true;
    for (var a = 0; a < arrayTemp.length; a++) {
        if (arrayTemp[a].length != 6) { isTrue = false; break; }
		else if(!parseInt(arrayTemp[a])){ isTrue = false; break; }
		else if(isNaN(parseInt(arrayTemp[a]))){ isTrue = false; break; }
    }
    if (isTrue == false) { alert("第一组号码不正确！"); return 0; }
    return arrayTemp.length;
};

/*******************************************************************************
*十一选五-后三直选复式
********************************************************************************/
var SYXW009 = function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split("|");
    if (arrayTemp.length != 3) { return 0; }
    var number = 0;
    var temp = arrayTemp[0].split(",");
    var temp1 = arrayTemp[1].split(",");
    var temp2 = arrayTemp[2].split(",");
    for (var k = 0; k < temp.length; k++) {
        for (var l = 0; l < temp1.length; l++) {
            for (var b = 0; b < temp2.length; b++) {
                if (temp[k] != temp1[l] && temp[k] != temp2[b] && temp1[l] != temp2[b]) 
				{
                    number=number+1;
                }
            }
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-中三组选单式
********************************************************************************/
var SYXW008 = function(chooseText){
    var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=6){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		number=number+1;
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;	
};

/*******************************************************************************
*十一选五-中三组选复式
********************************************************************************/
var SYXW007 = function(chooseText){
    if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 2) { return 0; }
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
            for (var k = j + 1; k < arrayTemp.length; k++) {
                number = number + 1;
            }
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-中三直选单式
********************************************************************************/
var SYXW006 = function(chooseText){
    var arrayTemp = chooseText.split(",");
    var isTrue = true;
    for (var a = 0; a < arrayTemp.length; a++) {
        if (arrayTemp[a].length != 6) { isTrue = false; break; }
		else if(!parseInt(arrayTemp[a])){ isTrue = false; break; }
		else if(isNaN(parseInt(arrayTemp[a]))){ isTrue = false; break; }
    }
    if (isTrue == false) { alert("第一组号码不正确！"); return 0; }
    return arrayTemp.length;
};

/*******************************************************************************
*十一选五-中三直选复式
********************************************************************************/
var SYXW005 = function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split("|");
    if (arrayTemp.length != 3) { return 0; }
    var number = 0;
    var temp = arrayTemp[0].split(",");
    var temp1 = arrayTemp[1].split(",");
    var temp2 = arrayTemp[2].split(",");
    for (var k = 0; k < temp.length; k++) {
        for (var l = 0; l < temp1.length; l++) {
            for (var b = 0; b < temp2.length; b++) {
                if (temp[k] != temp1[l] && temp[k] != temp2[b] && temp1[l] != temp2[b]) 
				{
                    number=number+1;
                }
            }
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-前三组选单式
********************************************************************************/
var SYXW004 = function(chooseText){
    var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=6){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		number=number+1;
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;	
};


/*******************************************************************************
*十一选五-前三直选复式
********************************************************************************/
var SYXW003 = function(chooseText){
    if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split(",");
    var number = 0;
    if (arrayTemp.length <= 2) { return 0; }
    for (var n = 0; n < arrayTemp.length; n++) {
        for (var j = n + 1; j < arrayTemp.length; j++) {
            for (var k = j + 1; k < arrayTemp.length; k++) {
                number = number + 1;
            }
        }
    }
    return number;
};

/*******************************************************************************
*十一选五-前三直选单式
********************************************************************************/
var SYXW002 = function(chooseText){
    var arrayTemp = chooseText.split(",");
    var isTrue = true;
    for (var a = 0; a < arrayTemp.length; a++) {
        if (arrayTemp[a].length != 6) { isTrue = false; break; }
		else if(!parseInt(arrayTemp[a])){ isTrue = false; break; }
		else if(isNaN(parseInt(arrayTemp[a]))){ isTrue = false; break; }
    }
    if (isTrue == false) { alert("第一组号码不正确！"); return 0; }
    return arrayTemp.length;
};

/*******************************************************************************
*十一选五-前三直选复式
********************************************************************************/
var SYXW001 = function(chooseText){
	if (chooseText.indexOf("-1") != -1) { return 0; }
    var arrayTemp = chooseText.split("|");
    if (arrayTemp.length != 3) { return 0; }
    var number = 0;
    var temp = arrayTemp[0].split(",");
    var temp1 = arrayTemp[1].split(",");
    var temp2 = arrayTemp[2].split(",");
    for (var k = 0; k < temp.length; k++) {
        for (var l = 0; l < temp1.length; l++) {
            for (var b = 0; b < temp2.length; b++) {
                if (temp[k] != temp1[l] && temp[k] != temp2[b] && temp1[l] != temp2[b]) 
				{
                    number=number+1;
                }
            }
        }
    }
    return number;
};

/*******************************************************************************
*PK10-定位胆
********************************************************************************/
var PK10007 = function(chooseText){
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=10){return 0;}
	var number=0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a]!="-1"){
			number=number+(parseInt(arrayTemp[a].split(",").length) || 0);
		}
	}
	return number;
};


/*******************************************************************************
*PK10-前二直选单式
********************************************************************************/
var PK10006 = function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=4)
		{isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*PK10-前二直选复式
********************************************************************************/
var PK10005 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var aTemp = arrayTemp[0].split(",");
	var bTemp = arrayTemp[1].split(",");
	var number=0;
	for(var a=0;a<aTemp.length;a++){
		for(var b=0;b<bTemp.length;b++){
			if(aTemp[a]!=bTemp[b]){
					number=number+1;
			}
		}
	}
	return number;
};
/*******************************************************************************
*PK10-前三直选单式
********************************************************************************/
var PK10004 = function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=6){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};
/*******************************************************************************
*PK10-前三直选复式
********************************************************************************/
var PK10003 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=3){return 0;}
	var aTemp = arrayTemp[0].split(",");
	var bTemp = arrayTemp[1].split(",");
	var cTemp = arrayTemp[2].split(",");
	var number=0;
	for(var a=0;a<aTemp.length;a++){
		for(var b=0;b<bTemp.length;b++){
			for(var c=0;c<cTemp.length;c++){
				if(aTemp[a]!=bTemp[b] && aTemp[a]!=cTemp[c] && bTemp[b]!=cTemp[c]){
					number=number+1;
				}
			}
		}
	}
	return number;
};

/*******************************************************************************
*PK10-前四直选单式
********************************************************************************/
var PK10002 = function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=8)
		{isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*PK10-前四直选复式
********************************************************************************/
var PK10001 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=4){return 0;}
	var aTemp = arrayTemp[0].split(",");
	var bTemp = arrayTemp[1].split(",");
	var cTemp = arrayTemp[2].split(",");
	var dTemp = arrayTemp[3].split(",");
	var number=0;
	for(var a=0;a<aTemp.length;a++){
		for(var b=0;b<bTemp.length;b++){
			for(var c=0;c<cTemp.length;c++){
				for(var d=0;d<dTemp.length;d++){
					if(aTemp[a]!=bTemp[b] && aTemp[a]!=cTemp[c] 
					&& cTemp[a]!=dTemp[d] && bTemp[b] != cTemp[c] 
					&& bTemp[b]!=dTemp[d] && cTemp[c]!=dTemp[d])
					{
						number=number+1;
					}
				}
			}
		}
	}
	return number;
};

/*******************************************************************************
*时时彩任选三星单式
********************************************************************************/
var SSC026 = function(chooseText,back){
	var iLength = 0;
	var Position = "";
	try{
		$("#frm-choose-position").find("input[name=frm]").each(function(){
			if(this.checked){iLength++;Position=Position+""+$(this).val();}
		});
	}catch(err){}
	if(parseInt(iLength)!=3){alert('请选择下注位数！');return 0;}
	else if(iLength==3){
		var arrayTemp = chooseText.split(",");
		var isTrue = true;
		var number =0;
		for(var a=0;a<arrayTemp.length;a++){
			if(arrayTemp[a].length!=0 && arrayTemp[a].length!=3){isTrue=false;break;}
			number=number+1;
		}
		if(isTrue==false){alert("第一组号码不正确！");return 0;}
		if(back!=undefined && typeof(back)=='function'){back("位数"+Position+","+chooseText);}
		return number;	
	}else{alert('第一组号码格式错误！');return 0;}
};

/*******************************************************************************
*时时彩前二直选复式
********************************************************************************/
var SSC027 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var number=1;
	for(var a=0;a<arrayTemp.length;a++){
		number=number*arrayTemp[a].split(",").length;
	}
	return number;		
};

/*******************************************************************************
*时时彩后二直选复式
********************************************************************************/
var SSC028 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length!=2){return 0;}
	var number=1;
	for(var a=0;a<arrayTemp.length;a++){
		number=number*arrayTemp[a].split(",").length;
	}
	return number;		
};

/*******************************************************************************
*时时彩前二直选单式
********************************************************************************/
var SSC029 = function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=2){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		else if(isNaN(parseInt(arrayTemp[a]))){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};
/*******************************************************************************
*时时彩后二直选单式
********************************************************************************/
var SSC030 = function(chooseText){
	var arrayTemp = chooseText.split(",");
	var isTrue = true;
	var number =0;
	for(var a=0;a<arrayTemp.length;a++){
		if(arrayTemp[a].length!=0 && arrayTemp[a].length!=2){isTrue=false;break;}
		else if(!parseInt(arrayTemp[a])){isTrue=false;break;}
		else if(isNaN(parseInt(arrayTemp[a]))){isTrue=false;break;}
		else{number=number+1;}
	}
	if(isTrue==false){alert("第一组号码不正确！");return 0;}
	return number;
};

/*******************************************************************************
*时时彩前二组选二
********************************************************************************/
var SSC031 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split(",");
	var number=0;
	if(arrayTemp.length<=1){return 0;}
	for(var n=0;n<arrayTemp.length;n++){
		for(var j=n+1;j<arrayTemp.length;j++){
			number=number+1;
		}	
	};
	return number;
};

/*******************************************************************************
*时时彩后二组选二
********************************************************************************/
var SSC032 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var arrayTemp = chooseText.split(",");
	var number=0;
	if(arrayTemp.length<=1){return 0;}
	for(var n=0;n<arrayTemp.length;n++){
		for(var j=n+1;j<arrayTemp.length;j++){
			number=number+1;
		}	
	};
	return number;
};

/*******************************************************************************
*时时彩任选二星复式
********************************************************************************/
var SSC033 = function(chooseText){
	chooseText = chooseText.replace(/\|-1/g,"").replace(/-1\|/g,"");
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length<=1){return 0;}
	try{
		var aLength=0,bLength=0,cLength=0,dLength=0,eLength=0
		aLength = arrayTemp[0].split(",").length;
		bLength = arrayTemp[1].split(",").length;
		var number=0;
		if(arrayTemp.length>=3){cLength = arrayTemp[2].split(",").length;}
		if(arrayTemp.length>=4){dLength = arrayTemp[3].split(",").length;}
		if(arrayTemp.length>=5){eLength = arrayTemp[4].split(",").length;}
		number=number + (aLength * bLength);
		number=number + (aLength * cLength);
		number=number + (aLength * dLength);
		number=number + (aLength * eLength);
		number=number + (bLength * cLength);
		number=number + (bLength * dLength);
		number=number + (bLength * eLength);
		number=number + (cLength * dLength);
		number=number + (cLength * eLength);
		number=number + (dLength * eLength);
		return number;
	}catch(err){return 0;}
};

/*******************************************************************************
*时时彩任选二星单式
********************************************************************************/
var SSC034 = function(chooseText,back){
	var iLength = 0;
	var Position = "";
	try{
	$("#frm-choose-position").find("input[name=frm]").each(function(){
		if(this.checked){iLength++;Position=Position+""+$(this).val();}
	});
	if(parseInt(iLength)!=2){alert('请选择下注位数！');return 0;}	
	}catch(err){}
	if(iLength==2){
		var arrayTemp = chooseText.split(",");
		var isTrue = true;
		var number =0;
		for(var a=0;a<arrayTemp.length;a++){
			if(arrayTemp[a].length!=0 && arrayTemp[a].length!=2){isTrue=false;break;}
			number=number+1;
		}
		if(isTrue==false){alert("第一组号码不正确！");return 0;}
		if(back!=undefined && typeof(back)=='function'){back("位数"+Position+","+chooseText);}
		return number;	
	}else{alert('第一组号码格式错误！');return 0;}
};
/*******************************************************************************
*时时彩,任选二星和值
********************************************************************************/
var SSC035 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var MaxNumber = 9;
		var number = 0;
		var arrayTemp = chooseText.split(",");
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					if((a+b)!=length){continue;}
					number=number+1;
				}	
			}
		};
		return number;		
	}catch(err){}
};

/*******************************************************************************
*时时彩前二和值
********************************************************************************/
var SSC036 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var MaxNumber = 9;
		var number = 0;
		var arrayTemp = chooseText.split(",");
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					if((a+b)!=length){continue;}
					number=number+1;
				}	
			}
		};
		return number;		
	}catch(err){}
};

/*******************************************************************************
*时时彩后二和值
********************************************************************************/
var SSC037 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var MaxNumber = 9;
		var number = 0;
		var arrayTemp = chooseText.split(",");
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					if((a+b)!=length){continue;}
					number=number+1;
				}	
			}
		};
		return number;		
	}catch(err){}
};

/*******************************************************************************
*时时彩前二和尾
********************************************************************************/
var SSC038 = function(chooseText){
	try{
		var arrayTemp = chooseText.split(",");
		var number = arrayTemp.length * 1;
		return number;
	}catch(err){}
};

/*******************************************************************************
*时时彩后二和尾
********************************************************************************/
var SSC039 = function(chooseText){
	try{
		var arrayTemp = chooseText.split(",");
		var number = arrayTemp.length * 1;
		return number;
	}catch(err){}
};

/*******************************************************************************
*时时彩定位胆
********************************************************************************/
var SSC040 = function(chooseText){
	try{
		var arrayTemp = chooseText.split("|");
		var number=0;
		for(var a=0;a<arrayTemp.length;a++){
			if(arrayTemp[a].indexOf("-1")!=-1){continue;}
			number=number+arrayTemp[a].split(",").length;
		}
		return number;	
	}catch(err){}
};


/*******************************************************************************
*时时彩五星不定位
********************************************************************************/
var SSC041 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var arrayTemp = chooseText.split(",");
		var number=arrayTemp.length;
		return number;	
	}catch(err){}
};

/*******************************************************************************
*时时彩前三不定位
********************************************************************************/
var SSC042 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var arrayTemp = chooseText.split(",");
		var number=arrayTemp.length;
		return number;	
	}catch(err){}
};

/*******************************************************************************
*时时彩中三不定位
********************************************************************************/
var SSC043 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var arrayTemp = chooseText.split(",");
		var number=arrayTemp.length;
		return number;	
	}catch(err){}
};

/*******************************************************************************
*时时彩大小单双
********************************************************************************/
var SSC044 = function(chooseText){
	try{
		if(chooseText.indexOf("-1")!=-1){return 0;}
		var arrayTemp = chooseText.split("|");
		if(arrayTemp.length!=2){return 0;}
		var number=1;
		for(var a=0;a<arrayTemp.length;a++){
			number=number*arrayTemp[a].split(",").length;
		};
		return number;	
	}catch(err){}
};
/*******************************************************************************
*时时彩五星直选复式
********************************************************************************/
var SSC001 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==5){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩五星直选单式
********************************************************************************/
var SSC002 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=5){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩前四直选复式
********************************************************************************/
var SSC003 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==4){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩前四直选单式
********************************************************************************/
var SSC004 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=4){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩后四直选复式
********************************************************************************/
var SSC005 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==4){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩后四直选单式
********************************************************************************/
var SSC006 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=4){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩前三直选复式
********************************************************************************/
var SSC007 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==3){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩前三直选单式
********************************************************************************/
var SSC008 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=3){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩中三直选复式
********************************************************************************/
var SSC009 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==3){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩中三直选单式
********************************************************************************/
var SSC010 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=3){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩侯三直选复式
********************************************************************************/
var SSC011 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split("|");
		if(arrTemp.length==3){
			for(var k in arrTemp){
				if(arrTemp[k].indexOf("-1")!=-1){
					number = number * 0;
				}else{
					number = number * parseInt(arrTemp[k].split(",").length);
				}
			}
		}
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩后三直选单式
********************************************************************************/
var SSC012 = function(chooseText){
	var number = 1;
	try{
		var arrTemp = chooseText.split(",");
		var remainder = 1;
		for(var k in arrTemp){
			if(arrTemp[k].length!=3){remainder=0;break;}	
		}
		number = arrTemp.length * remainder;
	}catch(err){}
	return number;
}

/*******************************************************************************
*时时彩前三组选三
********************************************************************************/
var SSC013 = function(chooseText){
	var number=0;
	try{
	  if(chooseText.indexOf("-1")!=-1){return 0;}
	  var arrayTemp = chooseText.split(",");
	  var length = arrayTemp.length;
	  if(length<=1){return 0;}
	  var number = length*(length-1);
	}catch(err){}
  return number;
}

/*******************************************************************************
*时时彩中三组选三
********************************************************************************/
var SSC015 = function(chooseText)
{
	var number=0;
	try{
	  if(chooseText.indexOf("-1")!=-1){return 0;}
	  var arrayTemp = chooseText.split(",");
	  var length = arrayTemp.length;
	  if(length<=1){return 0;}
	  var number = length*(length-1);
	}catch(err){}
  return number;
}

/*******************************************************************************
*时时彩后三组选三
********************************************************************************/
var SSC017 = function(chooseText){
	var number=0;
	try{
	  if(chooseText.indexOf("-1")!=-1){return 0;}
	  var arrayTemp = chooseText.split(",");
	  var length = arrayTemp.length;
	  if(length<=1){return 0;}
	  var number = length*(length-1);
	}catch(err){}
  return number;
}

/*******************************************************************************
*时时彩前三组选六
********************************************************************************/
var SSC014 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		if(arrayTemp.length<=2){return 0;}
		for(var n=0;n<arrayTemp.length;n++){
			for(var j=n+1;j<arrayTemp.length;j++){
				for(var k=j+1;k<arrayTemp.length;k++){
					number=number+1;
				}	
			}	
		}
		return number;
	}catch(err){}
  return number;
}

/*******************************************************************************
*时时彩中三组选六
********************************************************************************/
var SSC016 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		if(arrayTemp.length<=2){return 0;}
		for(var n=0;n<arrayTemp.length;n++){
			for(var j=n+1;j<arrayTemp.length;j++){
				for(var k=j+1;k<arrayTemp.length;k++){
					number=number+1;
				}	
			}	
		}
		return number;
	}catch(err){}
  return number;
}

/*******************************************************************************
*时时彩后三组选六
********************************************************************************/
var SSC018 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		if(arrayTemp.length<=2){return 0;}
		for(var n=0;n<arrayTemp.length;n++){
			for(var j=n+1;j<arrayTemp.length;j++){
				for(var k=j+1;k<arrayTemp.length;k++){
					number=number+1;
				}	
			}	
		}
		return number;
	}catch(err){}
  return number;
}
/*******************************************************************************
*时时彩前三和值
********************************************************************************/
var SSC019 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	var MaxNumber = 9;
	try{
		var arrayTemp = chooseText.split(",");
		var number = 0;
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					for(var c=0;c<=length;c++){
						if(c>MaxNumber){continue;}
						if((a+b+c)!=length){continue;}
						number=number+1;
					}
				}
			}
		}
	}catch(err){}
	return number;	
}
/*******************************************************************************
*时时彩中三和值
********************************************************************************/
var SSC020 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	var MaxNumber = 9;
	try{
		var arrayTemp = chooseText.split(",");
		var number = 0;
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					for(var c=0;c<=length;c++){
						if(c>MaxNumber){continue;}
						if((a+b+c)!=length){continue;}
						number=number+1;
					}
				}
			}
		}
	}catch(err){}
	return number;	
}
/*******************************************************************************
*时时彩后三和值
********************************************************************************/
var SSC021 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	var MaxNumber = 9;
	try{
		var arrayTemp = chooseText.split(",");
		var number = 0;
		for(var k=0;k<arrayTemp.length;k++){
			var length = parseInt(arrayTemp[k]);
			for(var a=0;a<=length;a++){
				if(a>MaxNumber){continue;}
				for(var b=0;b<=length;b++){
					if(b>MaxNumber){continue;}
					for(var c=0;c<=length;c++){
						if(c>MaxNumber){continue;}
						if((a+b+c)!=length){continue;}
						number=number+1;
					}
				}
			}
		}
	}catch(err){}
	return number;	
}

/*******************************************************************************
*时时彩前三和尾
********************************************************************************/
var SSC022 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		for(var k in arrayTemp){
			var s = parseInt(arrayTemp[k]);
			if(s>=0 && s<=9){number=number+1;}
			else{number=0;break;}
		}
	}catch(err){}
	return number;	
}
/*******************************************************************************
*时时彩中三和尾
********************************************************************************/
var SSC023 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		for(var k in arrayTemp){
			var s = parseInt(arrayTemp[k]);
			if(s>=0 && s<=9){number=number+1;}
			else{number=0;break;}
		}
	}catch(err){}
	return number;	
}
/*******************************************************************************
*时时彩后三和尾
********************************************************************************/
var SSC024 = function(chooseText){
	if(chooseText.indexOf("-1")!=-1){return 0;}
	var number=0;
	try{
		var arrayTemp = chooseText.split(",");
		for(var k in arrayTemp){
			var s = parseInt(arrayTemp[k]);
			if(s>=0 && s<=9){number=number+1;}
			else{number=0;break;}
		}
	}catch(err){}
	return number;	
}
/*******************************************************************************
*时时彩任选三星
********************************************************************************/
var SSC025=function(chooseText){
	chooseText = chooseText.replace(/\|-1/g,"").replace(/-1\|/g,"");
	var arrayTemp = chooseText.split("|");
	if(arrayTemp.length<=2){return 0;}
	try{
		var aLength=0,bLength=0,cLength=0,dLength=0,eLength=0
		aLength = arrayTemp[0].split(",").length;
		bLength = arrayTemp[1].split(",").length;
		cLength = arrayTemp[2].split(",").length;
		var number=0;
		if(arrayTemp.length>=4){dLength = arrayTemp[3].split(",").length;}
		if(arrayTemp.length>=5){eLength = arrayTemp[4].split(",").length;}
		number=number + (aLength * bLength * cLength);
		number=number + (aLength * bLength * dLength);
		number=number + (aLength * bLength * eLength);
		number=number + (aLength * cLength * dLength);
		number=number + (aLength * cLength * eLength);
		number=number + (aLength * dLength * eLength);
		number=number + (bLength * cLength * dLength);
		number=number + (bLength * cLength * eLength);
		number=number + (bLength * dLength * eLength);
		number=number + (cLength * dLength * eLength);
		return number;
	}catch(err){return 0;}
};




var bonusLoading=function(options){
	try{
		var options = options || {};
		try{
			var thisDiscount = parseFloat(cfg["discount"]) || 0;
			var discount = parseFloat($("#frm-range").val()) || 0;
			discount = parseFloat(thisDiscount - discount).toFixed(1);
			var minbonus = parseFloat(options["minbonus"]) || 0;
			var maxbonus = parseFloat(options["maxbonus"]) || 0;
			var scalepoints = parseFloat(options["scalepoints"]) || 0;
			var bonus = parseFloat(minbonus + parseInt((scalepoints*discount)));
			if(bonus>=maxbonus){bonus=maxbonus};
			$("#frm-bonus").val(bonus);
		}catch(err){}
	}catch(err){}
}
/******************************************************************************
*直选复式
*******************************************************************************/
var ChooseDirect=function(options)
{
	var options = options || {};
	if(options["name"]!=undefined && options["name"]!="" && options["code"]!=undefined && options["code"]!=""){
		var strhtml = "<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		var arrTemp = options["name"].toString().split("|");
		var codeTemp = options["code"].toString().split(",");
		for(var j in arrTemp)
		{
			strhtml+="<tr Kid=\""+j+"\" class=\"hmenu\">";
			strhtml+="<th operate=\"name\">"+arrTemp[j]+"</th>";
			strhtml+="<td operate=\"quickselection\">";
			strhtml+="<a operate=\"all\">全</a>";
			strhtml+="<a operate=\"single\">单</a>"
			strhtml+="<a operate=\"double\">双</a>"
			strhtml+="<a operate=\"will\">大</a>"
			strhtml+="<a operate=\"small\">小</a>"
			strhtml+="<a operate=\"clear\">清</a>";
			strhtml+="</td>";
			strhtml+="</tr>";
			strhtml+="<tr Kvalue=\""+j+"\"  class=\"hback\">";
			strhtml+="<td colspan=\"2\" operate=\"code\">";
			for(var k in codeTemp)
			{
				strhtml+="<a number=\""+codeTemp[k]+"\">"+codeTemp[k]+"</a>";
			}
			strhtml+="</td>";
			strhtml+="</tr>";
		}
		if(options["tips"]!=undefined && options["tips"]!=""){
			strhtml+="<tr class=\"operback\">";
			strhtml+="<td colspan=\"3\" operate=\"descrption\">";
			strhtml+=options["tips"];
			strhtml+="</td>";
			strhtml+="</tr>";
		}
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
	}
};
/******************************************************************************
*时时彩大小单双额外样式
*******************************************************************************/
var ChooseSSC044 = function(options)
{
	var options = options || {};
	if(options["name"]!=undefined && options["name"]!="" && options["code"]!=undefined && options["code"]!=""){
		var strhtml = "<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		
		strhtml+="<tr Kid=\"0\" class=\"hmenu\">";
		strhtml+="<th operate=\"name\">十位(请选择大小)</th></tr>";
		strhtml+="<tr Kvalue=\"0\" class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<a class=\"Tkvalue\" number=\"大\"><span>大</span><br/><span>(01234)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"小\"><span>小</span><br/><span>(56789)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"单\"><span>单</span><br/><span>(13579)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"双\"><span>双</span><br/><span>(02468)</span></a>";
		strhtml+="</td>";
		strhtml+="</tr>";
		
		strhtml+="<tr Kid=\"0\" class=\"hmenu\">";
		strhtml+="<th operate=\"name\">个位(请选择大小)</th></tr>";
		strhtml+="<tr Kvalue=\"0\" class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<a class=\"Tkvalue\" number=\"大\"><span>大</span><br/><span>(01234)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"小\"><span>小</span><br/><span>(56789)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"单\"><span>单</span><br/><span>(13579)</span></a>";
		strhtml+="<a class=\"Tkvalue\" number=\"双\"><span>双</span><br/><span>(02468)</span></a>";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="<tr class=\"operback\">";
		strhtml+="<td colspan=\"3\" operate=\"descrption\">";
		strhtml+="从个、十位中的大小单双4种属性中各选1种属性，所选属性与开奖号码的属性相同即为中奖";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
	}		
};

/******************************************************************************
*时时彩大小单双额外样式
*******************************************************************************/
var ChooseKS007 = function(options)
{
	var options = options || {};
	if(options["name"]!=undefined && options["name"]!="" && options["code"]!=undefined && options["code"]!=""){
		var strhtml = "<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		
		strhtml+="<tr Kid=\"0\" class=\"hmenu\">";
		strhtml+="<th operate=\"name\">选择同号</th></tr>";
		strhtml+="<tr Kvalue=\"0\" class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<a number=\"11\">11</a>";
		strhtml+="<a number=\"22\">22</a>";
		strhtml+="<a number=\"33\">33</a>";
		strhtml+="<a number=\"44\">44</a>";
		strhtml+="<a number=\"55\">55</a>";
		strhtml+="<a number=\"66\">66</a>";
		strhtml+="</td>";
		strhtml+="</tr>";
		
		strhtml+="<tr Kid=\"0\" class=\"hmenu\">";
		strhtml+="<th operate=\"name\">选不同号</th></tr>";
		strhtml+="<tr Kvalue=\"0\" class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<a number=\"1\">1</a>";
		strhtml+="<a number=\"2\">2</a>";
		strhtml+="<a number=\"3\">3</a>";
		strhtml+="<a number=\"4\">4</a>";
		strhtml+="<a number=\"5\">5</a>";
		strhtml+="<a number=\"6\">6</a>";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="<tr class=\"operback\">";
		strhtml+="<td colspan=\"3\" operate=\"descrption\">";
		strhtml+="选择1个相同号码和1个不同号码投注，选号与开奖号码一致即中奖";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
	}		
};
/******************************************************************************
*时时彩大小单双额外样式
*******************************************************************************/
var ChooseKuaisan = function(options)
{
	var options = options || {};
	if(options["name"]!=undefined && options["name"]!="" 
	&& options["code"]!=undefined && options["code"]!="")
	{
		var strhtml = "<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		var arrTemp = options["name"].toString().split("|");
		var codeTemp = options["code"].toString().split(",");
		for(var j in arrTemp)
		{
			strhtml+="<tr Kid=\"0\" class=\"hmenu\">";
			strhtml+="<th operate=\"name\">"+arrTemp[j]+"</th></tr>";
			strhtml+="<tr Kvalue=\""+j+"\"  class=\"hback\">";
			strhtml+="<td colspan=\"2\" operate=\"code\">";
			for(var k in codeTemp)
			{
				strhtml+="<a class=\"KSvalue\" number=\""+codeTemp[k]+"\">";
				strhtml+=""+codeTemp[k]+"";
				strhtml+="</a>";
			}
			strhtml+="</td>";
			strhtml+="</tr>";
		}
		if(options["tips"]!=undefined && options["tips"]!=""){
			strhtml+="<tr class=\"operback\">";
			strhtml+="<td operate=\"descrption\">";
			strhtml+=options["tips"];
			strhtml+="</td>";
			strhtml+="</tr>";
		};
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
	}		
};


/******************************************************************************
*时时彩任选二星单式
*******************************************************************************/
var ChooseSSC034 = function(options)
{
	var options = options || {};
	var strhtml = "";
	if(options["name"]!=undefined && options["name"]!="" 
	&& options["length"]!=undefined && options["length"]!="")
	{
		strhtml+="<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		strhtml+="<tr class=\"hback\">";
		strhtml+="<td id=\"frm-choose-position\" operate=\"btns\">";
		strhtml+="<label><input type=\"checkbox\" value=\"1\" name=\"frm\" />万位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"2\" name=\"frm\" />千位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"3\" name=\"frm\" />百位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"4\" name=\"frm\" />时位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"5\" name=\"frm\" />个位</label>";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="<tr class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<textarea type=\"tel\" id=\"frm-unitaryCode\" contentEditable=\"true\"></textarea>";
		strhtml+="</td>";
		strhtml+="</tr>";
		if(options["tips"]!=undefined && options["tips"]!=""){
			strhtml+="<tr class=\"operback\">";
			strhtml+="<td operate=\"descrption\">";
			strhtml+=options["tips"];
			strhtml+="</td>";
			strhtml+="</tr>";
		};
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
		/***************************************************************************
		*定义选择事件信息
		****************************************************************************/
		$("#frm-choose-position").find("input[name=frm]").click(function(){
			try{
				var iLength = parseInt($("#frm-choose-position").find("input[name=frm]:checked").length) || 0;
				if(parseInt(iLength)>=3){alert('最多选择三位数号码！');return false;}
			}catch(err){}
		});
		
	};	
};
/******************************************************************************
*时时彩任选三星单式
*******************************************************************************/
var ChooseSSC026 = function(options)
{
	var options = options || {};
	var strhtml = "";
	if(options["name"]!=undefined && options["name"]!="" 
	&& options["length"]!=undefined && options["length"]!="")
	{
		strhtml+="<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		strhtml+="<tr class=\"hback\">";
		strhtml+="<td id=\"frm-choose-position\" operate=\"btns\">";
		strhtml+="<label><input type=\"checkbox\" value=\"1\" name=\"frm\" />万位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"2\" name=\"frm\" />千位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"3\" name=\"frm\" />百位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"4\" name=\"frm\" />时位</label>";
		strhtml+="<label><input type=\"checkbox\" value=\"5\" name=\"frm\" />个位</label>";
		strhtml+="</td>";
		strhtml+="</tr>";
		strhtml+="<tr class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<textarea type=\"tel\" id=\"frm-unitaryCode\" contentEditable=\"true\"></textarea>";
		strhtml+="</td>";
		strhtml+="</tr>";
		if(options["tips"]!=undefined && options["tips"]!=""){
			strhtml+="<tr class=\"operback\">";
			strhtml+="<td operate=\"descrption\">";
			strhtml+=options["tips"];
			strhtml+="</td>";
			strhtml+="</tr>";
		};
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
		/***************************************************************************
		*定义选择事件信息
		****************************************************************************/
		$("#frm-choose-position").find("input[name=frm]").click(function(){
			try{
				var iLength = parseInt($("#frm-choose-position").find("input[name=frm]:checked").length) || 0;
				if(parseInt(iLength)>=4){alert('最多选择三位数号码！');return false;}
			}catch(err){}
		});
		
	};	
};
/******************************************************************************
*直选单式
*******************************************************************************/
var ChooseUnitary=function(options)
{
	var options = options || {};
	if(options["name"]!=undefined && options["name"]!="" && options["length"]!=undefined && options["length"]!=""){
		var strhtml = "<table cellpadding=\"1\" cellspacing=\"0\" border=\"0\" id=\"frm-code\">";
		strhtml+="<tr class=\"hback\">";
		strhtml+="<td operate=\"code\">";
		strhtml+="<textarea type=\"tel\" id=\"frm-unitaryCode\" contentEditable=\"true\"></textarea>";
		strhtml+="</td>";
		strhtml+="</tr>";
		if(options["tips"]!=undefined && options["tips"]!=""){
			strhtml+="<tr class=\"operback\">";
			strhtml+="<td operate=\"descrption\">";
			strhtml+=options["tips"];
			strhtml+="</td>";
			strhtml+="</tr>";
		}
		strhtml+="</table>";
		$("#frmControl").html(strhtml);	
	}
}