# A2TestTask
Test task for A2 company

1. Для запросов к ресурсу использовал библиотеку GraphQL.Client\
https://www.nuget.org/packages/GraphQL.Client/5.0.0

2. Для взаимодействия с БД использовал библиотеку Dapper\
https://www.nuget.org/packages/Dapper

3. БД поднята в Docker\
MCR.MICROSOFT.COM/MSSQL/SERVER 2022-latest

4. Структура таблицы
```sql
USE [a2team]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[wooddeals](
	[dealNumber] [varchar](29) NOT NULL,
	[dealDate] [date] NULL,
	[sellerName] [varchar](512) NULL,
	[sellerInn] [varchar](12) NOT NULL,
	[buyerName] [varchar](512) NULL,
	[buyerInn] [nchar](12) NOT NULL,
	[woodVolumeSeller] [real] NOT NULL,
	[woodVolumeBuyer] [real] NOT NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [wooddeals_PK] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
```

5. Модели данных GraphQL и БД вынесены в отдельные классы

6. Взаимодействие с GraphQL и БД реализованы в виде примитивных сервисов. Функционал сервисов ограничен требованиями тестового задания.

7. Более менее сносно работает при запросе одной страницы на 1000 записей. При этом записи добавляются поштучно. Первый проход, где `dealNumber` ещё не заподнен ~7 секунд на тысячу записей. Второй проход, где каждая попытка записи в БД спотыкается о присутствующий ключь ~30 секунд на 1000 записей. Я уверен, что как минимум намудрил с индексами, как максимум - выбрал неправильный подход. Поле для экспериментов имеется, времени на это не имеется :) Также была реализована попытка добавления записей "пачкой" с автоматической проверкой наличия `dealNumber`-ов в таблице:
```sql
INSERT INTO a2team.dbo.wooddeals
    (sellerName, sellerInn, buyerName, buyerInn, dealDate, dealNumber, woodVolumeBuyer, woodVolumeSeller)
    SELECT
        ttm.sellerName, ttm.sellerInn, ttm.buyerName, ttm.buyerInn, ttm.dealDate, ttm.dealNumber, ttm.woodVolumeBuyer, ttm.woodVolumeSeller
    FROM
        a2team.dbo.wooddeals rjwn RIGHT JOIN ( VALUES
            (@SellerName, @SellerInn, @BuyerName, @BuyerInn, @DealDate, @DealNumber, @WoodVolumeBuyer, @WoodVolumeSeller)
        ) AS ttm (sellerName, sellerInn, buyerName, buyerInn, dealDate, dealNumber, woodVolumeBuyer, woodVolumeSeller)
        ON rjwn.dealNumber = ttm.dealNumber
    WHERE
        rjwn.dealNumber IS NULL;
```
Сначало работало лучше поштучной записи, потом после экспериментов с индексами поплохело. К сожалению не имею особого опыта администрирования БД, а то возможно придумал бы что-нибудь.

8. Не уверен, что достаточчно корректно организовал работу с исключениями, сделал первое, что пришло в голову, кое-где, уверен, надо дополнительных проверок наставить, но опять же, на это не хватает времени на данный момент.

9. Трудозатраты - 16 часов с учетом того, что с GraphQL был не знаком, SQL запросы пришлось долго подбирать и тестить, а также вспоминать администрирование БД хоть в каком-то виде, по работе из администрирования в основном только добавление/удаление полей таблиц, большая часть времени ушла на попытки увеличить производительность приложения.

10. С уважением, Иван.
