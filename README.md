# Sqlinfo

## **✨ 项目简介**

🦄 **Sqlinfo是一款快速探测数据库信息工具**

项目地址：https://github.com/lele8/Sqlinfo

在内网渗透中快速获取数据库所有库名，表名，列名；具体判断后再去翻数据，节省时间。

工具支持mysql，mssql，PostgreSQL，redis并解析fscan扫描结果进行批量探测获取。

## **🚀 快速使用**

```
Sqlinfo.exe -f result.txt
Sqlinfo.exe -h ip -u username -p password -mysql
Sqlinfo.exe -h ip -u username -p password -mssql
Sqlinfo.exe -h ip -u username -p password -psql
Sqlinfo.exe -h ip -p password -redis
```

![](./image/print.png)

## 📜 免责声明

本工具仅面向**合法授权**的企业安全建设行为，如您需要测试本工具的可用性，请自行搭建靶机环境。

在使用本工具进行检测时，您应确保该行为符合当地的法律法规，并且已经取得了足够的授权。**请勿对非授权目标进行扫描。**

如您在使用本工具的过程中存在任何非法行为，您需自行承担相应后果，作者将不承担任何法律及连带责任。

在安装并使用本工具前，请您**务必审慎阅读、充分理解各条款内容**，限制、免责条款或者其他涉及您重大权益的条款可能会以加粗、加下划线等形式提示您重点注意。 除非您已充分阅读、完全理解并接受本协议所有条款，否则，请您不要安装并使用本工具。您的使用行为或者您以其他任何明示或者默示方式表示接受本协议的，即视为您已阅读并同意本协议的约束。

# **😽 鸣谢**

https://github.com/uknowsec/SharpSQLDump
