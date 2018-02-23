/*
Navicat MySQL Data Transfer

Source Server         : localhost_3306
Source Server Version : 100129
Source Host           : localhost:3306
Source Database       : test1

Target Server Type    : MYSQL
Target Server Version : 100129
File Encoding         : 65001

Date: 2018-02-24 03:54:17
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for account
-- ----------------------------
DROP TABLE IF EXISTS `account`;
CREATE TABLE `account` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`IsD`  tinyint(1) NOT NULL ,
`CreateTime`  datetime NOT NULL ,
`UpdateTime`  datetime NOT NULL ,
`AccountName`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`pwd`  varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Type`  int(11) NOT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=2

;

-- ----------------------------
-- Records of account
-- ----------------------------
BEGIN;
INSERT INTO `account` VALUES ('1', '0', '2018-02-08 12:05:35', '2018-02-08 12:05:38', 'admin', '123456', '0');
COMMIT;

-- ----------------------------
-- Table structure for qq_account
-- ----------------------------
DROP TABLE IF EXISTS `qq_account`;
CREATE TABLE `qq_account` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`IsD`  tinyint(1) NOT NULL ,
`CreateTime`  datetime NOT NULL ,
`UpdateTime`  datetime NOT NULL ,
`QQNum`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`pwd`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`StatusDesc`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`IsLogin`  tinyint(1) NOT NULL ,
`State`  int(11) NOT NULL ,
`Type`  int(11) NOT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=8738

;

-- ----------------------------
-- Records of qq_account
-- ----------------------------
BEGIN;
INSERT INTO `qq_account` VALUES ('1', '0', '2018-02-24 02:40:05', '2018-02-24 02:40:08', '1446426971', '2436741994', null, '0', '0', '0'), ('3', '0', '2018-02-24 02:40:16', '2018-02-24 02:40:19', '1', '1', '', '0', '0', '0');
COMMIT;

-- ----------------------------
-- Table structure for qq_friend
-- ----------------------------
DROP TABLE IF EXISTS `qq_friend`;
CREATE TABLE `qq_friend` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`IsD`  tinyint(1) NOT NULL ,
`CreateTime`  datetime NOT NULL ,
`UpdateTime`  datetime NOT NULL ,
`QQNum`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Nick`  varchar(100) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Owner`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=70

;



-- ----------------------------
-- Table structure for receivced_message
-- ----------------------------
DROP TABLE IF EXISTS `receivced_message`;
CREATE TABLE `receivced_message` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`IsD`  tinyint(1) NOT NULL ,
`CreateTime`  datetime NOT NULL ,
`UpdateTime`  datetime NOT NULL ,
`Type`  int(11) NOT NULL ,
`From`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`To`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Owner`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`GroupNum`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Message`  varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=25

;



-- ----------------------------
-- Table structure for sended_message
-- ----------------------------
DROP TABLE IF EXISTS `sended_message`;
CREATE TABLE `sended_message` (
`Id`  int(11) NOT NULL AUTO_INCREMENT ,
`IsD`  tinyint(1) NOT NULL ,
`CreateTime`  datetime NOT NULL ,
`UpdateTime`  datetime NOT NULL ,
`Type`  int(11) NOT NULL ,
`From`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`To`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Owner`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`GroupNum`  varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`Message`  varchar(5000) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL ,
`IsSend`  tinyint(1) NOT NULL ,
PRIMARY KEY (`Id`)
)
ENGINE=InnoDB
DEFAULT CHARACTER SET=utf8 COLLATE=utf8_general_ci
AUTO_INCREMENT=453

;


-- ----------------------------
-- Auto increment value for account
-- ----------------------------
ALTER TABLE `account` AUTO_INCREMENT=2;

-- ----------------------------
-- Auto increment value for qq_account
-- ----------------------------
ALTER TABLE `qq_account` AUTO_INCREMENT=8738;

-- ----------------------------
-- Auto increment value for qq_friend
-- ----------------------------
ALTER TABLE `qq_friend` AUTO_INCREMENT=70;

-- ----------------------------
-- Auto increment value for receivced_message
-- ----------------------------
ALTER TABLE `receivced_message` AUTO_INCREMENT=25;

-- ----------------------------
-- Auto increment value for sended_message
-- ----------------------------
ALTER TABLE `sended_message` AUTO_INCREMENT=453;
