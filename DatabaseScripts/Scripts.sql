USE [master]
GO
/****** Object:  Database [LMSPortal]    Script Date: 13/6/2025 8:49:41 AM ******/
CREATE DATABASE [LMSPortal]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'LMSPortal', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\LMSPortal.mdf' , SIZE = 532480KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'LMSPortal_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\LMSPortal_log.ldf' , SIZE = 270336KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [LMSPortal] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [LMSPortal].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [LMSPortal] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [LMSPortal] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [LMSPortal] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [LMSPortal] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [LMSPortal] SET ARITHABORT OFF 
GO
ALTER DATABASE [LMSPortal] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [LMSPortal] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [LMSPortal] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [LMSPortal] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [LMSPortal] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [LMSPortal] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [LMSPortal] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [LMSPortal] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [LMSPortal] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [LMSPortal] SET  DISABLE_BROKER 
GO
ALTER DATABASE [LMSPortal] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [LMSPortal] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [LMSPortal] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [LMSPortal] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [LMSPortal] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [LMSPortal] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [LMSPortal] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [LMSPortal] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [LMSPortal] SET  MULTI_USER 
GO
ALTER DATABASE [LMSPortal] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [LMSPortal] SET DB_CHAINING OFF 
GO
ALTER DATABASE [LMSPortal] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [LMSPortal] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [LMSPortal] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [LMSPortal] SET QUERY_STORE = OFF
GO
USE [LMSPortal]
GO
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO
ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO
ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO
USE [LMSPortal]
GO
/****** Object:  UserDefinedFunction [dbo].[Check_Number_Series]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Check_Number_Series](@Numbers nvarchar(100))
RETURNS nvarchar(100)
AS
BEGIN
    DECLARE @Result nvarchar(MAX)

    -- Replace commas with a space to handle comma-separated numbers
    SET @Numbers = REPLACE(@Numbers, ',', ' ')

    -- Split the string of numbers into individual values using space as the delimiter
    DECLARE @NumberTable TABLE (Number int)
    INSERT INTO @NumberTable (Number)
    SELECT CAST(value AS int)
    FROM STRING_SPLIT(@Numbers, ' ')

    DECLARE @DistinctCount INT

    -- Count the distinct values in the table
    SELECT @DistinctCount = COUNT(DISTINCT Number)
    FROM @NumberTable

    IF @DistinctCount = 1
    BEGIN
        -- If there is only one distinct value, all numbers are the same
        SELECT @Result = CAST(MAX(Number) AS NVARCHAR(MAX))
        FROM @NumberTable
    END
    ELSE
    BEGIN
        -- If there are multiple distinct values, the series is mixed
        SET @Result = 'ALL'
    END

    RETURN @Result
END
GO
/****** Object:  UserDefinedFunction [dbo].[Check_Store_Status]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Check_Store_Status](@Store_ID nvarchar(50))
RETURNS nvarchar(50)
AS
BEGIN
DECLARE @Status nvarchar(50)
SET @Status =  (    SELECT Top 1 CASE WHEN Is_Active != 1 
								 THEN CASE WHEN DATEDIFF(d, Inactive_Date, GETDATE()) > 90
									      THEN 'Closed'
					                      ELSE 'Suspended'
					                  END
			                     ELSE 'Active'   -- Status can be null for demo
			                     END
                    FROM  DMC_Store 
					WHERE Store_ID = @Store_ID
				)
RETURN @Status

END
GO
/****** Object:  UserDefinedFunction [dbo].[Currency_Conversion]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Currency_Conversion](@Ex_Date nvarchar(50), @Currency nvarchar(50), @Amount numeric(10,4))
RETURNS numeric(10,4)
AS
BEGIN
DECLARE @Result numeric(10,4)
DECLARE @Rate numeric(10,4)

SET @Rate = (CASE WHEN @Currency = 'SGD' THEN 1 ELSE (SELECT Top 1 CONVERT(decimal(10,4), Rate) FROM DB_Exchange_History WHERE [Date] = CAST(@Ex_Date AS date) AND Currency = @Currency) END)
SELECT @Result = @Rate * @Amount

RETURN @Result

END
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_Statistics]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_Statistics]
(
	@ReportName	 nvarchar(50), 
	@ReportMonth date
) 
RETURNS @Result TABLE
(
    Category   nvarchar(100),   -- will hold Country, Customer or Segment
    Stores     int,
    Total      decimal(10,0),
    Average    decimal(10,0)
)
AS
BEGIN
	IF @ReportName = 'ByCountry'
	BEGIN
		INSERT INTO @Result
		SELECT COALESCE(Country, 'Total') AS Category
			    , SUM(Owned_Store) AS Stores
				, CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
				, CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,0)) AS Average
		FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@ReportMonth)
		GROUP BY Country
		WITH ROLLUP;
	END
	ELSE IF @ReportName = 'ByCustomer'
	BEGIN
		WITH Classified AS
        (
            SELECT CASE WHEN Headquarter_Name LIKE '%sushiro%' THEN 'Sushiro'
                        WHEN Headquarter_Name LIKE '%chateraise%' THEN 'Chateraise'
                        ELSE 'Others'
                        END AS Customer
			     , Owned_Store
				 , Total_Amount_Per_Month
            FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@ReportMonth)
        )
        INSERT INTO @Result
        SELECT COALESCE(Customer, 'Total') AS Category
		     , SUM(Owned_Store) AS Stores
			 , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
			 , CAST(SUM(Total_Amount_Per_Month) / NULLIF(SUM(Owned_Store), 0) AS decimal(10,0)) AS Average
        FROM Classified
        GROUP BY Customer
        WITH ROLLUP;
	END
    ELSE IF @ReportName = 'BySegment'
    BEGIN
        INSERT INTO @Result
        SELECT COALESCE(Device_Type, 'Total') AS Category
		     , SUM(Owned_Store) AS Stores
			 , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
			 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,0)) AS Average
        FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@ReportMonth)
        GROUP BY Device_Type
        WITH ROLLUP;
    END

    RETURN;
END
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_Statistics_USD]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_Statistics_USD]
(
	@ReportName	 nvarchar(50), 
	@ReportMonth date
) 
RETURNS @Result TABLE
(
    Category   nvarchar(100),   -- will hold Country, Customer or Segment
    Stores     int,
    Total      decimal(10,0),
    Average    decimal(10,0)
)
AS
BEGIN
	IF @ReportName = 'ByCountry'
	BEGIN
		INSERT INTO @Result
		SELECT COALESCE(Country, 'Total') AS Category
			    , SUM(Owned_Store) AS Stores
				, CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
				, CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,0)) AS Average
		FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@ReportMonth)
		GROUP BY Country
		WITH ROLLUP;
	END
	ELSE IF @ReportName = 'ByCustomer'
	BEGIN
		WITH Classified AS
        (
            SELECT CASE WHEN Headquarter_Name LIKE '%sushiro%' THEN 'Sushiro'
                        WHEN Headquarter_Name LIKE '%chateraise%' THEN 'Chateraise'
                        ELSE 'Others'
                        END AS Customer
			     , Owned_Store
				 , Total_Amount_Per_Month
            FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@ReportMonth)
        )
        INSERT INTO @Result
        SELECT COALESCE(Customer, 'Total') AS Category
		     , SUM(Owned_Store) AS Stores
			 , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
			 , CAST(SUM(Total_Amount_Per_Month) / NULLIF(SUM(Owned_Store), 0) AS decimal(10,0)) AS Average
        FROM Classified
        GROUP BY Customer
        WITH ROLLUP;
	END
    ELSE IF @ReportName = 'BySegment'
    BEGIN
        INSERT INTO @Result
        SELECT COALESCE(Device_Type, 'Total') AS Category
		     , SUM(Owned_Store) AS Stores
			 , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
			 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,0)) AS Average
        FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@ReportMonth)
        GROUP BY Device_Type
        WITH ROLLUP;
    END

    RETURN;
END
GO
/****** Object:  UserDefinedFunction [dbo].[FormatLicenseCodeWithDashes]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[FormatLicenseCodeWithDashes](@License_code nvarchar(50))
RETURNS nvarchar(50)
AS
BEGIN
    SET @License_code = REPLACE(@License_code, '-', '') -- Remove existing dashes, if any

    RETURN CONCAT(
					LEFT(@license_code, 4),
					'-',
					SUBSTRING(@license_code, 5, 4),
					'-',
					SUBSTRING(@license_code, 9, 4),
					'-',
					SUBSTRING(@license_code, 13, 4),
					'-',
					RIGHT(@license_code, 4)
    )
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Account_Type]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Account_Type](@Code nvarchar(5))
RETURNS nvarchar(10)
AS
BEGIN
	DECLARE @Account_Type nvarchar(10)
	SET @Account_Type =  ( SELECT Name FROM Master_Account_Type WHERE Code = @Code )
	RETURN @Account_Type
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_AI_Licence_Activation_Key]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_AI_Licence_Activation_Key](@AI_Device_ID nvarchar(100))
RETURNS nvarchar(50)
AS
BEGIN
	DECLARE @ActivationKey nvarchar(50)
	SET @ActivationKey =  ( SELECT STRING_AGG([Licence Code], '	') WITHIN GROUP (ORDER BY [Activated Date] DESC) AS ActivationKey 
	                        FROM R_Activated_AI_Licence 
							WHERE [AI Device ID] = @AI_Device_ID )
	RETURN @ActivationKey
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_AI_Licence_Expiry_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_AI_Licence_Expiry_Date](@Licence_Code nvarchar(50))
--RETURNS date
RETURNS nvarchar(20)
AS
BEGIN
	--DECLARE @ExpiryDate date
	DECLARE @ExpiryDate nvarchar(20)
	SET @ExpiryDate =  ( SELECT MAX([Expired Date]) FROM R_Activated_AI_Licence WHERE [Licence Code] = @Licence_Code )
	RETURN @ExpiryDate
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_AI_Licence_Term]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_AI_Licence_Term](@Device_ID nvarchar(100))
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @LicenceTerm nvarchar(20)
	SET @LicenceTerm = ( SELECT TOP 1 [Licence Term] FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID ORDER BY CASE WHEN [Status] = 'Activated' THEN 1 ELSE 2 END )
	RETURN @LicenceTerm
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Distributor_Name]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Distributor_Name](@Customer_ID nvarchar(20))
RETURNS nvarchar(100)

AS
BEGIN
	DECLARE @Customer_Name nvarchar(100)
	SET @Customer_Name =  ( SELECT Name FROM Master_Customer WHERE Customer_ID = @Customer_ID )
	RETURN @Customer_Name
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Hardkey_Licence_Tier]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Hardkey_Licence_Tier](@PLU_Code nvarchar(10))
RETURNS nvarchar(100)
AS
BEGIN
	DECLARE @Tier_Group nvarchar(100)
	SET @Tier_Group =  ( SELECT Value_2 AS Tier_Group FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_4 = 'Hardkey Licence' AND Value_1 = @PLU_Code )
	RETURN @Tier_Group
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Licence_Activated_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Licence_Activated_Date](@Device_ID nvarchar(100))
RETURNS nvarchar(10)
AS
BEGIN
	DECLARE @Activated_Date nvarchar(20)
	SET @Activated_Date = ( SELECT TOP 1 [Activated Date] FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID )
	RETURN @Activated_Date
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Licence_Expiry_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Licence_Expiry_Date](@Device_ID nvarchar(100))
RETURNS nvarchar(10)
AS
BEGIN
	DECLARE @Expiry_Date nvarchar(20)
	SET @Expiry_Date = ( SELECT TOP 1 [Expired Date] FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID ORDER BY CASE WHEN [Status] = 'Activated' THEN 1 ELSE 2 END )
	RETURN @Expiry_Date
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Licence_Inv_Amount]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Licence_Inv_Amount](@Invoice_No nvarchar(30), @PO_No nvarchar(50))
RETURNS decimal(10,2)
AS
BEGIN
DECLARE @Total_Amount money
SET @Total_Amount =  ( SELECT SUM(Amount) FROM DB_Recovered_Invoice WHERE Invoice_No = @Invoice_No AND PO_No = @PO_No )
RETURN @Total_Amount

END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Licence_Inv_Currency]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Licence_Inv_Currency](@Invoice_No nvarchar(30))
RETURNS nvarchar(10)
AS
BEGIN
	DECLARE @Currency nvarchar(10)
	SET @Currency =  ( SELECT TOP 1 Currency FROM DB_Recovered_Invoice WHERE Invoice_No = @Invoice_No )
	RETURN @Currency
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Licence_Status]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Licence_Status](@Device_ID nvarchar(100))
RETURNS nvarchar(10)
AS
BEGIN
	DECLARE @Status nvarchar(20)
	SET @Status = CASE WHEN (SELECT TOP 1 Status FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID) IS NULL 
	                         AND (SELECT MAC_Addr FROM CZL_Licenced_Devices WHERE Device_ID = @Device_ID) IS NOT NULL
	                   THEN 'New'
					   ELSE CASE WHEN (SELECT TOP 1 Status FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID) IS NULL 
					             THEN 'Unknown' 
								 ELSE (SELECT TOP 1 Status FROM _LMS_Licence_Details WHERE [AI Device ID] = @Device_ID ORDER BY CASE WHEN [Status] = 'Activated' THEN 1 ELSE 2 END ) 
							     END 
					   END
	RETURN @Status
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Maintenance_Revenue_Median]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Maintenance_Revenue_Median](@ReportMonth As date, @Services_Group nvarchar(1)) RETURNS nvarchar(50)
AS
BEGIN
DECLARE @RevenueMedian decimal(10, 2)
SET @RevenueMedian =  ( SELECT (  
									( SELECT TOP 1 [Amount On Month]
									  FROM (
											 SELECT TOP 50 Percent [Amount On Month]
											 FROM dbo.Maintenance_Monthly_Revenue(@ReportMonth, @Services_Group)
									         ORDER BY [Amount On Month]
				                           ) AS A
			                          ORDER BY [Amount On Month] DESC ) 
									  +
		                            ( SELECT TOP 1 [Amount On Month]
			                          FROM (
					                         SELECT TOP 50 Percent [Amount On Month]
					                         FROM Maintenance_Monthly_Revenue(@ReportMonth, @Services_Group)
					                         WHERE [Amount On Month] IS NOT NULL AND [Customer ID] IS NOT NULL
					                         ORDER BY [Amount On Month] DESC
				                           ) AS B
			                          ORDER BY [Amount On Month] ASC )
		                        ) / 2
				       ) 
RETURN @RevenueMedian

END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_AI_Renewal_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_AI_Renewal_ID]() RETURNS nvarchar(50)
AS
BEGIN
DECLARE @NewRenewalID nvarchar(20)
DECLARE @NextSequence int

SET @NextSequence = ( SELECT TOP 1 CAST(SUBSTRING(Renewal_UID, 12, 4) As int) + 1
						 FROM LMS_AI_Licence_Renewal 
						 WHERE SUBSTRING(Renewal_UID, 5, 4) = YEAR(CAST(GETDATE() As date))	
						   AND SUBSTRING(Renewal_UID, 9, 2) = MONTH(CAST(GETDATE() As date))     
						 ORDER BY Renewal_UID DESC)

SET @NewRenewalID =  ( SELECT 'LRW-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + ISNULL(FORMAT(@NextSequence, 'd4'), FORMAT(1, 'd4')) )
RETURN @NewRenewalID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Customer_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Customer_ID]() 
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @NewCustomerID nvarchar(20)
	SET @NewCustomerID =  ( SELECT TOP 1 'CTR-' + FORMAT(SUBSTRING(Customer_ID, 5, 6) + 1, 'd6') FROM Master_Customer ORDER BY Customer_ID DESC )
	RETURN @NewCustomerID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_CZL_Account_Unique_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_CZL_Account_Unique_ID]() RETURNS nvarchar(50)
AS
BEGIN
	DECLARE @NewAccountUniqueID nvarchar(20)
	SET @NewAccountUniqueID =  ( SELECT TOP 1 'CZL-ACT-' + FORMAT(SUBSTRING(CZL_Account_Unique_ID, 9, 6) + 1, 'd6') FROM CZL_Account ORDER BY CZL_Account_Unique_ID DESC )
	RETURN @NewAccountUniqueID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_CZL_Licenced_Device_Unique_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_CZL_Licenced_Device_Unique_ID]() RETURNS nvarchar(50)
AS
BEGIN
DECLARE @NewUniqueID nvarchar(20)
DECLARE @NextSequence int

SET @NextSequence = ISNULL(( SELECT TOP 1 CAST(SUBSTRING(Unique_ID, 16, 4) As int) + 1
				      FROM CZL_Licenced_Devices 
					  WHERE SUBSTRING(Unique_ID, 9, 4) = YEAR(CAST(GETDATE() As date))	
				        AND SUBSTRING(Unique_ID, 13, 2) = MONTH(CAST(GETDATE() As date))     
					  ORDER BY Unique_ID DESC), 1)

SET @NewUniqueID =  ( SELECT 'CZL-DEV-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + ISNULL(FORMAT(@NextSequence, 'd4'), FORMAT(1, 'd4')) )
RETURN @NewUniqueID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Banner_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Banner_ID]() 
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @NewBannerID nvarchar(20)
	SET @NewBannerID =  CASE WHEN ( SELECT TOP 1 'BNR-' + FORMAT(SUBSTRING(Banner_ID, 5, 6) + 1, 'd6') FROM Maintenance_Banner ORDER BY Banner_ID DESC ) IS NULL 
	                         THEN 'BNR-000001'
							 ELSE ( SELECT TOP 1 'BNR-' + FORMAT(SUBSTRING(Banner_ID, 5, 6) + 1, 'd6') FROM Maintenance_Banner ORDER BY Banner_ID DESC )
							 END
	RETURN @NewBannerID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Contract_Unique_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Contract_Unique_ID]()
RETURNS nvarchar(20)
AS
BEGIN
DECLARE @NewUID nvarchar(20)
SET @NewUID =  ( SELECT 'MID-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + 
								 ISNULL( FORMAT( ( SELECT TOP 1 CAST(SUBSTRING(Unique_ID, 12, 4) As int) + 1
												   FROM Maintenance_Contract
												   WHERE SUBSTRING(Unique_ID, 5, 4) = YEAR(CAST(GETDATE() As date))	
												     AND SUBSTRING(Unique_ID, 9, 2) = MONTH(CAST(GETDATE() As date))     
												   ORDER BY Unique_ID DESC
												  ), 'd4')
												  , FORMAT(1, 'd4')
											   ) 
				       ) -- end of statement
RETURN @NewUID

END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Customer_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Customer_ID]() 
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @NewCustomerID nvarchar(20)
	SET @NewCustomerID =  CASE WHEN ( SELECT TOP 1 'CTR-' + FORMAT(SUBSTRING(Customer_ID, 5, 6) + 1, 'd6') FROM Maintenance_Customer ORDER BY Customer_ID DESC ) IS NULL 
	                           THEN 'CTR-000001'
							   ELSE ( SELECT TOP 1 'CTR-' + FORMAT(SUBSTRING(Customer_ID, 5, 6) + 1, 'd6') FROM Maintenance_Customer ORDER BY Customer_ID DESC )
							   END
	RETURN @NewCustomerID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Product_Type_Unique_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Product_Type_Unique_ID]() 
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @NewUID nvarchar(20)
	SET @NewUID =  CASE WHEN ( SELECT TOP 1 'UID-' + FORMAT(CAST(SUBSTRING(UID, 5, 6) AS int) + 1, 'd6') FROM Maintenance_Product_Type ORDER BY UID DESC ) IS NULL 
	                         THEN 'UID-000001'
							 ELSE ( SELECT TOP 1 'UID-' + FORMAT(CAST(SUBSTRING(UID, 5, 6) AS int) + 1, 'd6') FROM Maintenance_Product_Type ORDER BY UID DESC )
							 END
	RETURN @NewUID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Product_Unique_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Product_Unique_ID]() RETURNS nvarchar(20)
AS
BEGIN
DECLARE @NewUniqueID nvarchar(20)
DECLARE @NextSequence int
SET @NextSequence = ISNULL(( SELECT TOP 1 CAST(SUBSTRING(Unique_ID, 13, 4) As int) + 1
				      FROM Maintenance_Product 
					  WHERE SUBSTRING(Unique_ID, 6, 4) = YEAR(CAST(GETDATE() As date))	
				        AND SUBSTRING(Unique_ID, 10, 2) = MONTH(CAST(GETDATE() As date))     
					  ORDER BY Unique_ID DESC), 1)

SET @NewUniqueID =  ( SELECT 'PROD-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + ISNULL(FORMAT(@NextSequence, 'd4'), FORMAT(1, 'd4')) )
RETURN @NewUniqueID

END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Maintenance_Store_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Maintenance_Store_ID]() 
RETURNS nvarchar(20)
AS
BEGIN
	DECLARE @NewStoreID nvarchar(20)
	SET @NewStoreID =  CASE WHEN ( SELECT TOP 1 'STR-' + FORMAT(SUBSTRING(Store_ID, 5, 6) + 1, 'd6') FROM Maintenance_Store ORDER BY Store_ID DESC ) IS NULL 
	                        THEN 'STR-000001'
							ELSE ( SELECT TOP 1 'STR-' + FORMAT(SUBSTRING(Store_ID, 5, 6) + 1, 'd6') FROM Maintenance_Store ORDER BY Store_ID DESC )
							END
	RETURN @NewStoreID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Subscription_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Subscription_ID]()
RETURNS nvarchar(50)

AS

BEGIN
DECLARE @NewSubscriptionID nvarchar(20)
DECLARE @NextStoreSequence int

SET @NextStoreSequence = ( SELECT TOP 1 CAST(SUBSTRING(Subscription_ID, 12, 4) As int) + 1
						   FROM DMC_Subscription 
						   WHERE SUBSTRING(Subscription_ID, 5, 4) = YEAR(CAST(GETDATE() As date))	
						     AND SUBSTRING(Subscription_ID, 9, 2) = MONTH(CAST(GETDATE() As date))     
						   ORDER BY Subscription_ID DESC)

SET @NewSubscriptionID =  ( SELECT 'CRT-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + ISNULL(FORMAT(@NextStoreSequence, 'd4'), FORMAT(1, 'd4')) )
RETURN @NewSubscriptionID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Subscription_Start_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Subscription_Start_Date](@Store_ID nvarchar(20))
RETURNS date

AS
BEGIN
DECLARE @SubscriptionStartDate date
--SET @SubscriptionStartDate = ( CASE WHEN dbo.Get_Subscription_End_Date(@Store_ID) IS NOT NULL -- Has existing subscription
--                                    THEN DATEADD(DAY, 1, dbo.Get_Subscription_End_Date(@Store_ID))
--                                    ELSE CASE WHEN EXISTS(SELECT TOP 1 * FROM DMC_Store WHERE Store_ID = @Store_ID AND Account_Type = '01') 
--									          THEN ( SELECT DATEADD(DAY, 1, Inactive_Date) FROM DMC_Store WHERE Store_ID = @Store_ID AND Account_Type = '01' )
--											  ELSE ( SELECT CASE WHEN CAST(DAY(GETDATE()) As int) > 15
--											                     THEN DATEADD(MONTH, 2, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
--																 ELSE DATEADD(MONTH, 1, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
--																 END )
--											  END
--									END )

SET @SubscriptionStartDate = ( CASE WHEN dbo.Get_Subscription_End_Date(@Store_ID) IS NOT NULL -- Has existing subscription
                                    THEN DATEADD(DAY, 1, dbo.Get_Subscription_End_Date(@Store_ID))
                                    ELSE CASE WHEN (SELECT MAX(End_Date) FROM DMC_Subscription WHERE SUBSTRING(Store_ID, 2, 6) = SUBSTRING(@Store_ID, 2, 6)) IS NOT NULL AND EXISTS(SELECT TOP 1 * FROM DMC_Subscription WHERE Store_ID = @Store_ID)  -- New subscribed store and has other existing store
									          THEN DATEADD(DAY, 1, (SELECT MAX(End_Date) FROM DMC_Subscription WHERE SUBSTRING(Store_ID, 2, 6) = SUBSTRING(@Store_ID, 2, 6)))
											  ELSE ( SELECT CASE WHEN CAST(DAY(GETDATE()) As int) > 15    -- Brand new store
											                     THEN DATEADD(MONTH, 2, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
																 ELSE DATEADD(MONTH, 1, DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0))
																 END ) 
											  END
										 
									END )

RETURN @SubscriptionStartDate
END

GO
/****** Object:  UserDefinedFunction [dbo].[Get_New_Termed_Licence_Renewal_ID]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_New_Termed_Licence_Renewal_ID]() RETURNS nvarchar(50)
AS
BEGIN
DECLARE @NewRenewalID nvarchar(20)
DECLARE @NextSequence int

SET @NextSequence = ( SELECT TOP 1 CAST(SUBSTRING(Renewal_UID, 13, 4) As int) + 1
						 FROM LMS_Termed_Licence_Renewal 
						 WHERE SUBSTRING(Renewal_UID, 6, 4) = YEAR(CAST(GETDATE() As date))	
						   AND SUBSTRING(Renewal_UID, 10, 2) = MONTH(CAST(GETDATE() As date))     
						 ORDER BY Renewal_UID DESC)

SET @NewRenewalID =  ( SELECT 'TLRW-' + CAST(YEAR(GETDATE()) As nvarchar) + FORMAT(MONTH(GETDATE()), 'd2') + '-' + ISNULL(FORMAT(@NextSequence, 'd4'), FORMAT(1, 'd4')) )
RETURN @NewRenewalID
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_NumberOfDays]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_NumberOfDays](@StartDate date, @EndDate date) RETURNS int
AS
BEGIN
	DECLARE @NumberOfDays int
	SET @NumberOfDays = DATEDIFF(DAY, @StartDate, @EndDate)
	RETURN @NumberOfDays
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_NumberOfMonth]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_NumberOfMonth](@StartDate date, @EndDate date) RETURNS int
AS
BEGIN
	DECLARE @NumberOfMonth int
	SET @NumberOfMonth = DATEDIFF(MONTH, DATEADD(DAY, -DAY(@StartDate) + 1, @StartDate), DATEADD(DAY, -DAY(@StartDate) + 1, @EndDate)) + 1
	RETURN @NumberOfMonth
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Revenue_Median]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Revenue_Median](@ReportMonth As date) RETURNS nvarchar(50)
AS
BEGIN
DECLARE @RevenueMedian decimal(10, 2)
SET @RevenueMedian =  ( SELECT (  
									( SELECT TOP 1 [Monthly_Fee]
									  FROM (
											 SELECT TOP 50 Percent [Monthly_Fee]
											 FROM dbo.DMC_Monthly_Subscription(@ReportMonth)
									         ORDER BY [Monthly_Fee]
				                           ) AS A
			                          ORDER BY [Monthly_Fee] DESC ) 
									  +
		                            ( SELECT TOP 1 [Monthly_Fee]
			                          FROM (
					                         SELECT TOP 50 Percent [Monthly_Fee]
					                         FROM dbo.DMC_Monthly_Subscription(@ReportMonth)
					                         WHERE [Monthly_Fee] IS NOT NULL AND [Customer] IS NOT NULL
					                         ORDER BY [Monthly_Fee] DESC
				                           ) AS B
			                          ORDER BY [Monthly_Fee] ASC )
		                        ) / 2
				       ) 
RETURN @RevenueMedian

END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Special_Arranged_Bill_Entity]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Special_Arranged_Bill_Entity](@Subscription_ID nvarchar(20))
RETURNS nvarchar(20)

AS
BEGIN
DECLARE @Store_ID nvarchar(20)
DECLARE @BillEntityID nvarchar(100)

SET @Store_ID = (SELECT TOP 1 Store_ID FROM DMC_Subscription WHERE Subscription_ID = @Subscription_ID)

-- If a subscription has a reassign bill entity record, then get the bill entity name
-- Otherwise, get the last set bill entity from previous reassign bill entity record
-- if no record, then return NULL
SET @BillEntityID = CASE WHEN EXISTS(SELECT DISTINCT Arranged_Bill_Entity FROM DB_Bill_Entity_Special_Arranged WHERE Subscription_ID = @Subscription_ID)
						 THEN (SELECT DISTINCT Arranged_Bill_Entity FROM DB_Bill_Entity_Special_Arranged WHERE Subscription_ID = @Subscription_ID)
						 ELSE (SELECT TOP 1 Arranged_Bill_Entity FROM DB_Bill_Entity_Special_Arranged A
                               LEFT JOIN DMC_Subscription B ON B.Subscription_ID = A.Subscription_ID
							   WHERE Store_ID = @Store_ID ORDER BY A.Subscription_ID DESC)
						 END

RETURN @BillEntityID

END 
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Subscriber_Group]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Subscriber_Group](@Subscription_ID nvarchar(20))
RETURNS nvarchar 
AS
BEGIN
	DECLARE @Subscriber_Group nvarchar
	SET @Subscriber_Group = ( SELECT TOP 1 Subscriber_Group FROM DMC_Subscription WHERE Subscription_ID = @Subscription_ID )
	RETURN @Subscriber_Group
END
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Subscription_End_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Subscription_End_Date](@Store_ID nvarchar(20))
RETURNS date
AS
BEGIN
DECLARE @EndDate date
SET @EndDate =  ( SELECT Top 1 End_Date FROM DMC_Subscription
				  WHERE Store_ID = @Store_ID --AND Payment_Status NOT IN ('Cancelled')
                  ORDER BY End_Date Desc	 
			    ) 
RETURN @EndDate
END

GO
/****** Object:  UserDefinedFunction [dbo].[Get_Subscription_Start_Date]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Subscription_Start_Date](@Store_ID nvarchar(20))
RETURNS date
AS
BEGIN
DECLARE @StartDate date
SET @StartDate =  ( SELECT Top 1 Start_Date FROM DMC_Subscription
					WHERE Store_ID = @Store_ID AND Payment_Status NOT IN ('Cancelled')
					ORDER BY End_Date Desc	 -- Subscription start date of latest subscription
				  ) 
RETURN @StartDate
END

GO
/****** Object:  UserDefinedFunction [dbo].[Monthly_Avg_Exchange_Rate]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Monthly_Avg_Exchange_Rate](@ReportMonth date, @Currency nvarchar(50)) RETURNS numeric(10,6)
AS
BEGIN
DECLARE @Result numeric(10,6)
DECLARE @AVGRate numeric(10,6)

SET @AVGRate = ( CASE WHEN @Currency = 'SGD' 
                      THEN 1
					  ELSE CASE WHEN ( SELECT TOP 1 [Rate] FROM DB_Exchange_History WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -1, @ReportMonth))) AND @ReportMonth AND Currency = @Currency ORDER BY [Date] DESC ) IS NOT NULL    -- if the exchange rate data is available then used the average
					            THEN ( SELECT TOP 1 [Rate] FROM DB_Exchange_History WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -1, @ReportMonth))) AND @ReportMonth AND Currency = @Currency ORDER BY [Date] DESC )
								ELSE CASE WHEN @Currency = 'USD' THEN 1.350000
								          WHEN @Currency = 'EUR' THEN 1.500000
								          END
								END
					  END
				)  -- Get the average rate of previous month

--SET @AVGRate = ( CASE WHEN @Currency = 'SGD' 
--                      THEN 1
--					  ELSE CASE WHEN ( SELECT AVG(Rate) FROM DB_Exchange_History WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -2, @ReportMonth))) AND EOMONTH(DATEADD(M, -1, @ReportMonth)) AND Currency = @Currency ) IS NOT NULL    -- if the exchange rate data is available then used the average
--					            THEN ( SELECT AVG(Rate) FROM DB_Exchange_History WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -2, @ReportMonth))) AND EOMONTH(DATEADD(M, -1, @ReportMonth)) AND Currency = @Currency )
--								ELSE CASE WHEN @Currency = 'USD' THEN 1.350000
--								          WHEN @Currency = 'EUR' THEN 1.500000
--								          END
--								END
--					  END
--				)  -- Get the average rate of previous month

RETURN @AVGRate

END
GO
/****** Object:  UserDefinedFunction [dbo].[Monthly_Avg_Exchange_Rate_Base_USD]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Monthly_Avg_Exchange_Rate_Base_USD](@ReportMonth date, @Currency nvarchar(50)) RETURNS numeric(10,6)
AS
BEGIN
DECLARE @Result numeric(10,6)
DECLARE @AVGRate numeric(10,6)

SET @AVGRate = ( CASE WHEN @Currency = 'USD' 
                      THEN 1
					  ELSE CASE WHEN ( SELECT TOP 1 [Rate] FROM DB_Exchange_History_Base_USD WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -1, @ReportMonth))) AND @ReportMonth AND Currency = @Currency ORDER BY [Date] DESC ) IS NOT NULL    -- if the exchange rate data is available then used the average
					            THEN ( SELECT TOP 1 [Rate] FROM DB_Exchange_History_Base_USD WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -1, @ReportMonth))) AND @ReportMonth AND Currency = @Currency ORDER BY [Date] DESC )
								ELSE CASE WHEN @Currency = 'SGD' THEN 0.7300
								          WHEN @Currency = 'EUR' THEN 0.8900
								          END
								END
					  END
				)  -- Get the average rate of previous month

--SET @AVGRate = ( CASE WHEN @Currency = 'USD' 
--                      THEN 1
--					  ELSE CASE WHEN ( SELECT AVG(Rate) FROM DB_Exchange_History_Base_USD WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -2, @ReportMonth))) AND EOMONTH(DATEADD(M, -1, @ReportMonth)) AND Currency = @Currency ) IS NOT NULL    -- if the exchange rate data is available then used the average
--					            THEN ( SELECT AVG(Rate) FROM DB_Exchange_History_Base_USD WHERE [Date] BETWEEN DATEADD(d, 1, EOMONTH(DATEADD(M, -2, @ReportMonth))) AND EOMONTH(DATEADD(M, -1, @ReportMonth)) AND Currency = @Currency )
--								ELSE CASE WHEN @Currency = 'SGD' THEN 0.7300
--								          WHEN @Currency = 'EUR' THEN 0.8900
--								          END
--								END
--					  END
--				)  -- Get the average rate of previous month

RETURN @AVGRate

END
GO
/****** Object:  Table [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_DMC_Subscription_Revenue_By_Account_Type_Base_USD_Overview]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_DMC_Subscription_Revenue_By_Account_Type_Base_USD_Overview]
AS
SELECT [Year]
     , [COL]
	 , ISNULL([Jan], 0) AS [Jan]
	 , ISNULL([Feb], 0) AS [Feb]
	 , ISNULL([Mar], 0) AS [Mar]
	 , ISNULL([Apr], 0) AS [Apr]
	 , ISNULL([May], 0) AS [May]
	 , ISNULL([Jun], 0) AS [Jun]
	 , ISNULL([Jul], 0) AS [Jul]
	 , ISNULL([Aug], 0) AS [Aug]
	 , ISNULL([Sep], 0) AS [Sep]
	 , ISNULL([Oct], 0) AS [Oct]
	 , ISNULL([Nov], 0) AS [Nov]
	 , ISNULL([Dec], 0) AS [Dec]
	 , CASE WHEN [COL] = 'Amount' THEN (ISNULL([Jan], 0) + ISNULL([Feb], 0) + ISNULL([Mar], 0) + ISNULL([Apr], 0) + ISNULL([May], 0) + ISNULL([Jun], 0) + ISNULL([Jul], 0) + ISNULL([Aug], 0) + ISNULL([Sep], 0) + ISNULL([Oct], 0) + ISNULL([Nov], 0) + ISNULL([Dec], 0)) ELSE 0 END AS Total
FROM (
		SELECT [Year], [Month], COL, VAL FROM TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary
        CROSS APPLY (VALUES('Amount', Total_Amount), ('No of store', CAST(No_Of_Store AS int)), ('Average', Average)) CS (COL, VAL)) T
        PIVOT (MAX([VAL]) FOR [Month] IN ([Jan], [Feb], [Mar], [Apr], [May], [Jun], [Jul], [Aug], [Sep], [Oct], [Nov], [Dec])) PVT
WHERE YEAR(GETDATE()) - [Year] <= 4
GO
/****** Object:  Table [dbo].[DMC_Store]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Store](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[Banner] [nvarchar](100) NULL,
	[Zone] [nvarchar](50) NULL,
	[Account_Type] [nvarchar](5) NOT NULL,
	[Created_Date] [date] NULL,
	[Effective_Date] [date] NULL,
	[Public_IP] [nvarchar](20) NULL,
	[FTP_Host] [nvarchar](30) NULL,
	[FTP_User] [nvarchar](20) NULL,
	[FTP_Password] [nvarchar](20) NULL,
	[Is_Active] [bit] NULL,
	[Inactive_Date] [date] NULL,
	[Last_Updated] [date] NULL,
	[Synced_dmcstore_id] [nvarchar](5) NULL,
	[Synced_dmcstore_userstoreid] [nvarchar](5) NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Synced_dmcstore_saleslastuseddate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Subscription]    Script Date: 13/6/2025 8:49:41 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Subscription](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Subscription_ID] [nvarchar](20) NOT NULL,
	[Start_Date] [date] NOT NULL,
	[End_Date] [date] NULL,
	[Duration] [nvarchar](20) NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Payment_Method] [nvarchar](20) NULL,
	[Payment_Mode] [nvarchar](20) NULL,
	[Ref_Invoice_No] [nvarchar](30) NULL,
	[Invoiced_Date] [date] NULL,
	[Payment_Status] [nvarchar](20) NULL,
	[Subscriber_Group] [nvarchar](1) NULL,
	[Store_ID] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Master_Customer_Group]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Master_Customer_Group](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Group_ID] [nvarchar](5) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Master_Sales_Representative]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Master_Sales_Representative](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Sales_Representative_ID] [nvarchar](10) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](30) NULL,
	[Short_Name] [nvarchar](50) NULL,
	[Supported_By] [nvarchar](10) NULL,
	[Supervised_By] [nvarchar](10) NULL,
	[Is_Active] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Headquarter]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Headquarter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Created_Date] [date] NULL,
	[Is_Active] [bit] NULL,
	[Inactive_Date] [date] NULL,
	[Customer_ID] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Headquarter_Sales_Representative]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Headquarter_Sales_Representative](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
	[Effective_Date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Master_Account_Type]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Master_Account_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](5) NULL,
	[Name] [nvarchar](10) NOT NULL,
	[Description] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Master_Customer]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Master_Customer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](20) NULL,
	[Address] [nvarchar](255) NULL,
	[Country] [nvarchar](50) NULL,
	[By_Distributor] [nvarchar](20) NULL,
	[Group_ID] [nvarchar](10) NULL,
	[Distributor_Code] [nvarchar](10) NULL,
	[Created_Date] [date] NULL,
	[Is_Active] [bit] NULL,
	[Inactive_Date] [date] NULL,
	[Contact_Person] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[_DMC_All_Active_Subscription]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_DMC_All_Active_Subscription]
AS

	SELECT [Subscription ID]
		 , ISNULL(dbo.Get_Distributor_Name(dbo.Get_Special_Arranged_Bill_Entity(TBL.[Subscription ID])), [Bill Entity]) AS [Bill Entity]
		 , [Customer ID], [Customer Name], [Group]
		 , [HQ Code], [HQ Name]
		 , [Store Code], [Store Name]
		 , [Created Date]
		 , [Start Date], [End Date], [Duration], [Currency], [Fee], [Status]
		 , [Account Type], [Sales Representative]
	FROM (
			 SELECT (SELECT TOP 1 Subscription_ID FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Subscription ID]
					, CASE WHEN C.By_Distributor = '' THEN C.Name 
						ELSE (Select Name From Master_Customer Where Customer_ID = C.By_Distributor) END AS [Bill Entity]
					, C.Customer_ID AS [Customer ID]
					, C.Name AS [Customer Name] 
					, G.Name AS [Group]
					, H.Headquarter_ID AS [HQ Code]
					, H.Name AS [HQ Name]
					, CASE WHEN S.Synced_dmcstore_userstoreid IS NOT NULL THEN CAST(Synced_dmcstore_userstoreid AS int) ELSE CAST(SUBSTRING(S.Store_ID, 8, 4) As int) END AS [Store Code]
					, S.Name AS [Store Name]
					, S.Created_Date AS [Created Date]
					, (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Start Date]
					, (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [End Date]
					, (SELECT TOP 1 Duration FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Duration]
					, (SELECT TOP 1 Currency FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Currency]
					, (SELECT TOP 1 Fee FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Fee]
					, CASE WHEN S.Is_Active = 1 THEN 'Active' ELSE 'In-Active' END AS [Status]
					, T.Name AS [Account Type]
					, (SELECT TOP 1 MR.Name 
					FROM DMC_Headquarter_Sales_Representative R
					INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
					WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
					ORDER BY R.Effective_Date DESC) AS [Sales Representative]
			FROM DMC_Store S
			INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
			INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
			INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
			INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
			WHERE S.Account_Type IN ('03') AND S.Is_Active = 1
	) TBL

GO
/****** Object:  Table [dbo].[Server_Space]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Server_Space](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Reading_Date] [datetime] NULL,
	[Size] [decimal](10, 1) NULL,
	[Used] [decimal](10, 1) NULL,
	[Avail] [decimal](10, 1) NULL,
	[Used_Percentage] [decimal](10, 0) NULL,
	[Remarks] [nvarchar](200) NULL,
	[DB_Size] [decimal](10, 0) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[_Server_Space_Quarter]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Quarter]
AS
	SELECT YEAR(Reading_Date) AS [Year]
		 , ((MONTH(Reading_Date) - 1) / 3) + 1 AS [Quarter No]
	     , DATEFROMPARTS(YEAR(MIN(CAST(Reading_Date AS date))), MONTH(MIN(CAST(Reading_Date AS date))), 1) AS [Start Date]
		 , EOMONTH(MAX(CAST(Reading_Date AS date))) AS [End Date]
		 , MAX(Size) AS [Server Space]
		 , MAX(Used) AS [Used]
		 , MAX(Avail) AS [Available]
		 , MAX(Used_Percentage) AS [Usage]
		 , MAX(DB_Size) AS [DB Size]
	FROM Server_Space
	WHERE DB_Size > 0
	GROUP BY YEAR(Reading_Date), ((MONTH(Reading_Date) - 1) / 3) + 1
GO
/****** Object:  Table [dbo].[DB_Lookup]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Lookup](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Lookup_Name] [nvarchar](100) NOT NULL,
	[Value_1] [nvarchar](100) NULL,
	[Value_2] [nvarchar](100) NULL,
	[Value_3] [nvarchar](100) NULL,
	[Value_4] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Customer]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Customer](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Address] [nvarchar](255) NULL,
	[Created_Date] [date] NULL,
	[Is_Active] [bit] NULL,
	[Last_Updated] [date] NULL,
	[Services_Group] [nvarchar](1) NULL,
	[Contact_Person] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Store]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Store](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Store_Name] [nvarchar](50) NOT NULL,
	[Created_Date] [date] NULL,
	[Last_Updated] [date] NULL,
	[Is_Active] [bit] NULL,
	[Banner_ID] [nvarchar](20) NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Contract]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Contract](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Unique_ID] [nvarchar](20) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Start_Date] [date] NULL,
	[End_Date] [date] NULL,
	[Currency] [nvarchar](3) NULL,
	[Amount] [money] NULL,
	[Reference_No] [nvarchar](30) NULL,
	[Status_Code] [nvarchar](3) NULL,
	[Invoice_No] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_Contract]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Maintenance_Contract]
AS
SELECT A.Unique_ID AS [Unique ID]
     , A.Customer_ID AS [Customer ID]
	 , C.Name AS [Customer Name]
	 , A.Store_ID AS [Store ID]
	 , B.Store_Name AS [Store Name]
	 , A.Start_Date AS [Start Date]
	 , A.End_Date AS [End Date]
	 , CASE WHEN (DATEDIFF(YEAR, A.Start_Date, A.End_Date) * 12) < 1 THEN 12 ELSE (DATEDIFF(YEAR, A.Start_Date, A.End_Date) * 12) END AS [Period]
	 , A.Currency AS [Currency]
	 , A.Amount AS [Amount]
	 , CASE WHEN A.Currency = 'SGD' THEN A.Currency ELSE 'SGD' END AS [Base Currency]
	 , CASE WHEN A.Currency = 'SGD' THEN ISNULL(A.Amount, 0) ELSE Round(CAST(ISNULL(dbo.Currency_Conversion(A.Start_Date, A.Currency, A.Amount), 0) AS money), 2) END AS [Base Currency Amount]
	 , A.Reference_No AS [Reference No]
	 --, YEAR(A.Start_Date) AS [FY]
	 , CASE WHEN YEAR(A.End_Date) - YEAR(A.Start_Date) = 0 THEN YEAR(A.Start_Date)
	        ELSE YEAR(A.End_Date)
			END AS [FY]
	 , C.Services_Group AS [Services Group]
     --, CASE WHEN DATEADD(Month, -6, GETDATE()) >= A.Start_Date THEN 1 ELSE 0 END AS [In Used]
	 , CASE WHEN YEAR(GETDATE()) > YEAR(A.Start_Date) THEN 1 ELSE 0 END AS [In Used]
	 , Status_Code AS [Status Code]
	 , (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Contract Process Status' AND Value_2 = Status_Code) AS [Status]
	 , ISNULL(Invoice_No, '') AS [Invoice No]
FROM Maintenance_Contract A
LEFT JOIN Maintenance_Store B ON B.Store_ID = A.Store_ID AND B.Customer_ID = A.Customer_ID
LEFT JOIN Maintenance_Customer C ON C.Customer_ID = A.Customer_ID
GO
/****** Object:  Table [dbo].[CZL_Licenced_Devices]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Licenced_Devices](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Unique_ID] [nvarchar](20) NOT NULL,
	[Device_Serial] [nvarchar](20) NULL,
	[Device_ID] [nvarchar](100) NULL,
	[Model] [nvarchar](10) NULL,
	[AI_Software_Version] [nvarchar](10) NULL,
	[R_Version] [nvarchar](10) NULL,
	[Scale_SN] [nvarchar](50) NULL,
	[MAC_Addr] [nvarchar](50) NULL,
	[Production_Licence_No] [nvarchar](50) NULL,
	[Location] [nvarchar](100) NULL,
	[Created_Date] [date] NULL,
	[Last_Updated] [date] NULL,
	[Client_ID] [nvarchar](5) NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NULL,
	[Effective_Date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CZL_Account]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NOT NULL,
	[Client_ID] [nvarchar](5) NULL,
	[Created_Date] [date] NULL,
	[User_Group] [nvarchar](50) NULL,
	[By_Distributor] [nvarchar](20) NULL,
	[Gen_Version] [nvarchar](20) NULL,
	[Country] [nvarchar](50) NULL,
	[Effective_Date] [date] NULL,
	[One_Year_Period_End] [date] NULL,
	[Account_Model] [nvarchar](10) NULL,
	[AI_Gateway_Key] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_CZL_Device_Model_Mismatched_With_Account_Model]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_CZL_Device_Model_Mismatched_With_Account_Model]
AS
	SELECT CASE WHEN B.By_Distributor != '' THEN C.Name ELSE '' END AS [Distributor] 
         , B.Client_ID AS [Account ID]
         , B.User_Group AS [Account Name]
	     , A.Device_Serial AS [Device Serial]
	     , A.Device_ID AS [Device ID]
	     , A.Scale_SN AS [Scale SN]
	     , A.Location AS [Location]
	     , A.Model AS [Device Model]
	     , B.Account_Model AS [Account Model]
    FROM CZL_Licenced_Devices A
    INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID
    LEFT JOIN Master_Customer C ON C.Customer_ID = B.By_Distributor
    WHERE A.Model != B.Account_Model
GO
/****** Object:  View [dbo].[_Server_Space_Semiannual]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Semiannual]
AS
	SELECT YEAR(Reading_Date) AS [Year]
		 , ((MONTH(Reading_Date) - 1) / 6) + 1 AS [Semiannual]
		 , DATEFROMPARTS(YEAR(MIN(CAST(Reading_Date AS date))), MONTH(MIN(CAST(Reading_Date AS date))), 1) AS [Start Date]
		 , EOMONTH(MAX(CAST(Reading_Date AS date))) AS [End Date]
		 , MAX(Size) AS [Server Space]
		 , MAX(Used) AS [Used]
		 , MAX(Avail) AS [Available]
		 , MAX(Used_Percentage) AS [Usage]
		 , MAX(DB_Size) AS [DB Size]
	FROM Server_Space
	WHERE DB_Size > 0
	GROUP BY YEAR(Reading_Date), ((MONTH(Reading_Date) - 1) / 6) + 1
GO
/****** Object:  View [dbo].[_Server_Space_Year]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Year]
AS
	SELECT YEAR(Reading_Date) AS [Year]
		 , DATEFROMPARTS(YEAR(MIN(CAST(Reading_Date AS date))), MONTH(MIN(CAST(Reading_Date AS date))), 1) AS [Start Date]
		 , EOMONTH(MAX(CAST(Reading_Date as date))) AS [End Date]
		 , MAX(Size) AS [Server Space]
		 , MAX(Used) AS [Used]
		 , MAX(Avail) AS [Available]
		 , MAX(Used_Percentage) AS [Usage]
		 , MAX(DB_Size) AS [DB Size]
	FROM Server_Space
	WHERE DB_Size > 0
	GROUP BY YEAR(Reading_Date)
GO
/****** Object:  View [dbo].[_AccountUsage]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_AccountUsage]
AS
	SELECT C.Name AS Customer_Name, G.Name AS [Group], H.Headquarter_ID, H.Name AS Headquarter_Name
		 , CAST(SUBSTRING(S.Store_ID, 8, 4) AS int) AS Store_Code, S.Name AS Store_Name, S.Created_Date
		 --, S.Inactive_Date
		 , CASE WHEN dbo.Check_Store_Status(S.Store_ID) != 'Active' THEN ISNULL(S.Inactive_Date, dbo.Get_Subscription_End_Date(S.Store_ID)) ELSE NULL END AS Inactive_Date
		 , dbo.Check_Store_Status(S.Store_ID) AS Status
		 , CASE WHEN dbo.Get_Account_Type(S.Account_Type) = 'Test' THEN 'Demo' ELSE dbo.Get_Account_Type(S.Account_Type) END AS Account_Type
		 , S.Synced_dmcstore_saleslastuseddate AS Last_Used_Date
	FROM DMC_Store S
	LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
	LEFT JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
	LEFT JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
GO
/****** Object:  View [dbo].[_AccountUsage_Overview]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_AccountUsage_Overview]
AS
	SELECT TBL1.[Group], TBL1.Account_Type, TBL2.Active_Within_30_Days, TBL3.Active_Within_180_Days, TBL4.Active_Within_365_Days, TBL5.Last_Active_More_Than_365_Days, TBL6.Account_No_Activity, TBL1.Total
	FROM (
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Total FROM _AccountUsage
			GROUP BY [Group], Account_Type
		 ) TBL1
	LEFT JOIN (		
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Active_Within_30_Days FROM _AccountUsage
			WHERE DATEDIFF(d, Last_Used_Date, GETDATE()) <= 30
			GROUP BY [Group], Account_Type
			  ) TBL2 ON TBL2.[Group] = TBL1.[Group] AND TBL2.Account_Type = TBL1.Account_Type
	LEFT JOIN (
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Active_Within_180_Days FROM _AccountUsage
			WHERE DATEDIFF(d, Last_Used_Date, GETDATE()) > 30 AND DATEDIFF(d, Last_Used_Date, GETDATE()) <= 180
			GROUP BY [Group], Account_Type
			  ) TBL3 ON TBL3.[Group] = TBL1.[Group] AND TBL3.Account_Type = TBL1.Account_Type
	LEFT JOIN (
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Active_Within_365_Days FROM _AccountUsage
			WHERE DATEDIFF(d, Last_Used_Date, GETDATE()) > 180 AND DATEDIFF(d, Last_Used_Date, GETDATE()) <= 365
			GROUP BY [Group], Account_Type
			  ) TBL4 ON TBL4.[Group] = TBL1.[Group] AND TBL4.Account_Type = TBL1.Account_Type
	LEFT JOIN (
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Last_Active_More_Than_365_Days FROM _AccountUsage
			WHERE DATEDIFF(d, Last_Used_Date, GETDATE()) > 365
			GROUP BY [Group], Account_Type
			  ) TBL5 ON TBL5.[Group] = TBL1.[Group] AND TBL5.Account_Type = TBL1.Account_Type
	LEFT JOIN (
			SELECT [Group], Account_Type, COUNT(Account_Type) AS Account_No_Activity FROM _AccountUsage
			WHERE Last_Used_Date IS NULL
			GROUP BY [Group], Account_Type
			  ) TBL6 ON TBL6.[Group] = TBL1.[Group] AND TBL6.Account_Type = TBL1.Account_Type
GO
/****** Object:  View [dbo].[_DMC_All_Subscribed_Accounts]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_DMC_All_Subscribed_Accounts]
AS
SELECT C.Name AS Customer
     , G.Name AS [Group]
	 , H.Headquarter_ID AS HQ_Code
	 , H.Name AS HQ_Name
	 , SUBSTRING(S.Store_ID, 8, 4) AS Store_No
	 , S.Name AS Store_Name
	 , S.Created_Date AS Created_Date
	 , (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS Start_Date
	 , (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS End_Date
	 , (SELECT TOP 1 Duration FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS Duration
	 , (SELECT TOP 1 Currency FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS Currency
	 , (SELECT TOP 1 Fee FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS Fee
	 , CASE WHEN S.Is_Active != 1 
	        THEN CASE WHEN DATEDIFF(d, (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC), GETDATE()) <= 90 AND (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) IS NOT NULL
			          THEN 'Suspended' ELSE 'Closed' END
			ELSE 'Active' END AS Status
	 , T.Name AS Account_Type
	 , (SELECT TOP 1 MR.Name FROM DMC_Headquarter_Sales_Representative R
		INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
		WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
		ORDER BY R.Effective_Date DESC) AS Requested_By
	 , S.Store_ID
FROM DMC_Store S
INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID

GO
/****** Object:  Table [dbo].[LMS_Hardkey_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Hardkey_Licence](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[SO_No] [nvarchar](10) NULL,
	[SO_Date] [date] NULL,
	[Invoice_No] [nvarchar](50) NULL,
	[Invoice_Date] [date] NULL,
	[Licence_No] [nvarchar](30) NULL,
	[PLU_Code] [nvarchar](10) NULL,
	[Created_Date] [date] NULL,
	[Start_Date] [date] NULL,
	[End_Date] [date] NULL,
	[Prepared_By] [nvarchar](50) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[I_LMS_Hardkey_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_LMS_Hardkey_Licence]
AS
	SELECT H.Customer_ID
		 , C.Name
		 , H.PO_No
		 , H.PO_Date
		 , H.SO_No
		 , H.SO_Date
		 , H.Invoice_No
		 , H.Invoice_Date
		 , H.Prepared_By
		 , H.Licence_No
		 , H.Created_Date
		 , dbo.Get_Hardkey_Licence_Tier(H.PLU_Code) AS Licence_Tier
		 , S.Name AS Requested_By 
	FROM LMS_Hardkey_Licence H
	INNER JOIN Master_Customer C on C.Customer_ID = H.Customer_ID
	INNER JOIN Master_Sales_Representative S on S.Sales_Representative_ID = H.Sales_Representative_ID
GO
/****** Object:  Table [dbo].[DB_SO_No_By_PO]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_SO_No_By_PO](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[SO_No] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[I_DB_SO_No_By_PO]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_DB_SO_No_By_PO]
AS
	SELECT A.Customer_ID AS [Customer ID]
	     , B.Name AS [Name]
		 , STRING_AGG(A.Sales_Representative_ID, ', ') AS [Requestor ID]
		 , STRING_AGG(C.Name, ', ') AS [Requested By]
		 , A.PO_No AS [PO No]
		 , A.PO_Date AS [PO Date]
		 , A.SO_No AS [SO No]
	FROM [DB_SO_No_By_PO] A
	INNER JOIN Master_Customer B ON B.Customer_ID = A.Customer_ID
	INNER JOIN Master_Sales_Representative C ON C.Sales_Representative_ID = A.Sales_Representative_ID
	GROUP BY A.Customer_ID, B.Name, A.PO_No, A.PO_Date, A.SO_No
GO
/****** Object:  Table [dbo].[Maintenance_Product_Type]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Product_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UID] [nvarchar](20) NOT NULL,
	[Code] [nvarchar](20) NOT NULL,
	[Product_Name] [nvarchar](100) NOT NULL,
	[Category] [nvarchar](20) NULL,
	[Services_Group] [nvarchar](1) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Product]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Product](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Unique_ID] [nvarchar](20) NOT NULL,
	[Serial_No] [nvarchar](20) NOT NULL,
	[Created_Date] [date] NOT NULL,
	[Installation_Date] [date] NOT NULL,
	[Usage_Start_Date] [date] NOT NULL,
	[Warranty_Expiration] [date] NOT NULL,
	[Value_Currency] [nvarchar](3) NOT NULL,
	[Product_Value] [money] NOT NULL,
	[Last_Updated] [date] NOT NULL,
	[Product_Code] [nvarchar](20) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Banner]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Banner](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Banner_ID] [nvarchar](20) NOT NULL,
	[Banner_Name] [nvarchar](50) NOT NULL,
	[Created_Date] [date] NULL,
	[Last_Updated] [date] NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Frequency] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_Store]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_Store]
AS
	SELECT Store_ID AS [Store ID]
	     , Store_Name AS [Store Name]
		 , Created_Date AS [Created Date]
		 , Last_Updated AS [Last Updated]
		 , CASE WHEN Is_Active = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
		 , Banner_ID AS [Banner ID]
		 , (SELECT TOP 1 Banner_Name FROM Maintenance_Banner WHERE Banner_ID = Maintenance_Store.Banner_ID) AS [Banner Name]
		 , Customer_ID AS [Customer ID]
		 , (SELECT TOP 1 Services_Group FROM Maintenance_Customer WHERE Customer_ID = Maintenance_Store.Customer_ID) AS [Services Group]
		 , CASE WHEN (SELECT COUNT(Store_ID) FROM Maintenance_Product WHERE Store_ID = Maintenance_Store.Store_ID) > 0 OR (SELECT COUNT(Store_ID) FROM Maintenance_Contract WHERE Store_ID = Maintenance_Store.Store_ID) > 0 THEN 1 ELSE 0 END AS [In Used]
	FROM Maintenance_Store
GO
/****** Object:  View [dbo].[R_Maintenance_Product]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[R_Maintenance_Product]
AS
	SELECT Unique_ID AS [Unique ID]
	     , Serial_No AS [Serial No]
		 , Product_Code AS [Product Code]
		 , B.Product_Name AS [Product Name]
		 , B.Category AS [Category]
		 , Created_Date AS [Created Date]
		 , (SELECT TOP 1 [Banner Name] FROM R_Maintenance_Store WHERE [Store ID] = A.Store_ID AND [Status] = 'Active') AS [Banner]
		 , (SELECT TOP 1 [Store Name] FROM R_Maintenance_Store WHERE [Store ID] = A.Store_ID AND [Status] = 'Active') AS [Location]
		 , A.Installation_Date AS [Installation Date]
		 , Value_Currency AS [Currency]
		 , Product_Value AS [Product Value]
	     , (CASE WHEN Value_Currency = 'SGD' THEN Value_Currency ELSE 'SGD' END) AS [Base Currency]
	     , (CASE WHEN Value_Currency = 'SGD' THEN Product_Value ELSE Round(CAST(dbo.Currency_Conversion(Usage_Start_Date, Value_Currency, Product_Value) AS money), 2) END) AS [Base Currency Value]
		 , Usage_Start_Date AS [Usage Start Date]
		 , Warranty_Expiration AS [Warranty Expiration]
		 , DATEDIFF(Month, Usage_Start_Date, Warranty_Expiration) AS [Warranty Cover Period]
		 , CASE WHEN GETDATE() <= Warranty_Expiration THEN 'Yes' ELSE 'No' END AS [Under Warranty]
		 , Last_Updated AS [Last Updated]
		 , Store_ID AS [Store ID]
		 , Customer_ID AS [Customer ID]
		 --, (SELECT TOP 1 Services_Group FROM Maintenance_Customer WHERE Customer_ID = A.Customer_ID) AS [Services Group]
		 , B.Services_Group AS [Services Group]
		 , CASE WHEN DATEADD(Day, -7, GETDATE()) >= Usage_Start_Date THEN 1 ELSE 0 END AS [In Used]
	FROM Maintenance_Product A
	LEFT JOIN Maintenance_Product_Type B ON B.Code = A.Product_Code AND B.Services_Group = (SELECT TOP 1 Services_Group FROM Maintenance_Customer WHERE Customer_ID = A.Customer_ID)
GO
/****** Object:  Table [dbo].[LMS_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Licence](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Application_Type] [nvarchar](30) NULL,
	[OS_Type] [nvarchar](30) NULL,
	[Serial_No] [nvarchar](50) NULL,
	[Created_Date] [date] NULL,
	[Synced_dmcmobiletoken_unique_id] [nvarchar](150) NULL,
	[Synced_dmcmobiletoken_activateddate] [date] NULL,
	[Synced_dmcmobiletoken_expireddate] [date] NULL,
	[Synced_dmcmobiletoken_status] [nvarchar](20) NULL,
	[Licensee_Email] [nvarchar](100) NULL,
	[Chargeable] [bit] NULL,
	[Synced_dmcmobiletoken_term] [int] NULL,
	[Synced_dmcmobiletoken_maxhq] [int] NULL,
	[Synced_dmcmobiletoken_maxstore] [int] NULL,
	[Remarks] [nvarchar](200) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
	[AI_Device_ID] [nvarchar](100) NULL,
	[AI_Device_Serial_No] [nvarchar](20) NULL,
	[Is_Cancelled] [bit] NULL,
	[CZL_Client_ID] [nvarchar](5) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[I_LMS_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[I_LMS_Licence]
AS
SELECT L.Customer_ID AS [Customer ID]
     , C.Name AS [Licensee]
	 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
     , L.PO_No AS [PO No]
	 , L.PO_Date AS [PO Date]
	 , L.Invoice_No AS [Invoice No]
	 , L.Invoice_Date AS [Invoice Date]
	 , (SELECT Value_2 FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_4 IN ('App Licence', 'DMC Server Licence Key') AND Value_3 = L.Application_Type) AS [Application Type]
	 , L.Application_Type AS [DB App Type]
	 , L.OS_Type AS [OS Type]
	 , L.Licence_Code AS [Licence Code]
	 , L.Synced_dmcmobiletoken_term AS [Licence Term]
	 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
	 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
	 , L.Serial_No AS [Serial No]
	 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
	 , L.Created_Date AS [Created Date]
	 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
	 , CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
	        THEN 'No Expiry' 
			ELSE CONVERT(nvarchar, DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 23) 
			END AS [Expired Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		    THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				      THEN L.Synced_dmcmobiletoken_status
					  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
					  END
		    ELSE L.Synced_dmcmobiletoken_status
		    END AS [Status]
	 , L.Licensee_Email AS [Email]
	 , SR.Name AS [Requested By]
	 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
	 , L.Remarks 
	 , L.Is_Cancelled AS [Is Cancelled]
FROM LMS_Licence AS L 
INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
WHERE L.Application_Type NOT IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence') 
GO
/****** Object:  Table [dbo].[Maintenance_Contract_Status_Log]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Contract_Status_Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Created_Date] [date] NULL,
	[Log_Description] [nvarchar](max) NULL,
	[Unique_ID] [nvarchar](20) NULL,
	[Log_Type] [nvarchar](3) NULL,
	[Last_Update] [date] NULL,
	[By_Who] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_Contract_Status_Log]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Maintenance_Contract_Status_Log]
AS
SELECT ID, Created_Date, Log_Description, Unique_ID
     , CASE WHEN DATEDIFF(d, Created_Date, GETDATE()) > 90 THEN 'SYS' ELSE Log_Type END AS Log_Type
	 , Last_Update, By_Who
     , 'Added by ' + By_Who + ' (' + CAST(Created_Date AS nvarchar) + ')' AS Added_By
     , CASE WHEN DATEDIFF(d, Created_Date, Last_Update) > 0 THEN 1 ELSE 0 END AS IsEdited
	 , CASE WHEN Log_Type != 'SYS' 
	        THEN CASE WHEN DATEDIFF(d, Created_Date, Last_Update) > 0 THEN 'Edited by ' + By_Who + ' (' + CAST(Last_Update AS nvarchar) + ')' ELSE NULL END 
			ELSE NULL 
			END AS Edited_By
FROM Maintenance_Contract_Status_Log
GO
/****** Object:  Table [dbo].[CZL_Account_Setup_Charge]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Account_Setup_Charge](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NOT NULL,
	[Client_ID] [nvarchar](5) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CZL_Account_Model_Upgrade_Trail]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Account_Model_Upgrade_Trail](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NOT NULL,
	[From_Model] [nvarchar](10) NULL,
	[To_Model] [nvarchar](10) NULL,
	[Effective_Date] [date] NULL,
	[Bind_Key] [nvarchar](50) NULL,
	[UID] [nvarchar](20) NULL,
	[Last_Update] [date] NULL,
	[Remarks] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_CZL_Account]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_CZL_Account]
AS
SELECT C.CZL_Account_Unique_ID AS [CZL Account ID]
     , CAST(C.Client_ID AS Int) AS [CZL Client ID]
     , C.User_Group AS [User Group]
	 , C.Created_Date AS [Created Date]
	 , C.By_Distributor AS [Distributor ID]
	 , CASE WHEN C.By_Distributor != '' THEN D.Name ELSE '' END AS [By Distributor] 
	 , C.Country AS [Country]
	 , C.Gen_Version AS [Gen Version]
	 , (SELECT COUNT(*) FROM CZL_Licenced_Devices WHERE CZL_Account_Unique_ID = C.CZL_Account_Unique_ID) AS [No of Registered Device]
	 --, ISNULL((SELECT TOP 1 To_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = C.CZL_Account_Unique_ID ORDER BY Effective_Date DESC, To_Model DESC)
	 --       , (SELECT TOP 1 Model FROM CZL_Licenced_Devices WHERE CZL_Account_Unique_ID = C.CZL_Account_Unique_ID ORDER BY CAST(ISNULL(Model, 0) AS int) DESC)) AS Model
	 , ISNULL((SELECT TOP 1 To_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = C.CZL_Account_Unique_ID ORDER BY Effective_Date DESC, To_Model DESC), Account_Model) AS Model
     , (SELECT COUNT(*) AS NoOfUpdateWithinGracePeriod 
	    FROM CZL_Account_Model_Upgrade_Trail A 
	    INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID 
		WHERE A.Effective_Date < DATEADD(Year, 1, B.Effective_Date) AND A.CZL_Account_Unique_ID = C.CZL_Account_Unique_ID) AS [Grace Period Model Update]
     , (SELECT COUNT(*) AS NoOfUpdateAfterGracePeriod 
	    FROM CZL_Account_Model_Upgrade_Trail A 
	    INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID 
		WHERE A.Effective_Date >= DATEADD(Year, 1, B.Effective_Date) AND A.CZL_Account_Unique_ID = C.CZL_Account_Unique_ID) AS [After Grace Period Model Update]
FROM CZL_Account C
LEFT JOIN Master_Customer D ON D.Customer_ID = C.By_Distributor
GO
/****** Object:  View [dbo].[I_CZL_Account_Setup_Fee]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_CZL_Account_Setup_Fee]
AS
	SELECT [CZL Account ID]
	     , [CZL Client ID]
		 , [User Group] AS [Account Name]
		 , [Created Date]
		 , [Distributor ID]
		 , [By Distributor]
		 , [Country]
		 , [Gen Version]
		 , [No of Registered Device]
		 , PO_No AS [PO No]
		 , PO_Date AS [PO Date]
		 , Invoice_No AS [Invoice No]
		 , Invoice_Date AS [Invoice Date]
		 , A.Sales_Representative_ID AS [Requestor ID]
		 , S.Name AS [Requested By]
		 , Currency AS [Currency]
		 --, dbo.Get_Licence_Inv_Currency(Invoice_No) AS Currency
		 , Fee AS [Amount]
		 --, dbo.Get_Licence_Inv_Amount(Invoice_No, PO_No) AS Amount
	FROM CZL_Account_Setup_Charge A
	INNER JOIN R_CZL_Account B ON B.[CZL Account ID] = A.CZL_Account_Unique_ID AND B.[CZL Client ID] = A.Client_ID
	INNER JOIN Master_Sales_Representative S ON S.Sales_Representative_ID = A.Sales_Representative_ID
GO
/****** Object:  Table [dbo].[LMS_Module_Licence_Order]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Module_Licence_Order](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Created_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Remarks] [nvarchar](100) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
	[UID] [nvarchar](20) NULL,
	[Is_Cancelled] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_Termed_Licence_Renewal]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Termed_Licence_Renewal](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Renewal_UID] [nvarchar](20) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Renewal_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Remarks] [nvarchar](100) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_AI_Licence_Renewal]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_AI_Licence_Renewal](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Renewal_UID] [nvarchar](20) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Renewal_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Remarks] [nvarchar](100) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[_PO_No_Ref_Invoice_For_All_Type_Of_Request]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_PO_No_Ref_Invoice_For_All_Type_Of_Request]
AS
SELECT * FROM (	
	SELECT PO_No, Invoice_No, 'Hardkey Licence' AS Category FROM LMS_Hardkey_Licence
    UNION
    SELECT PO_No, Invoice_No, 'App Product Licence' AS Category FROM LMS_Licence WHERE Application_Type NOT IN ('PC Scale')
    UNION
    SELECT PO_No, Invoice_No, 'Module Licence' AS Category FROM LMS_Licence WHERE Application_Type IN ('PC Scale')
    UNION
	SELECT PO_No, Invoice_No, 'Module Licence' AS Category FROM LMS_Module_Licence_Order
    UNION
    SELECT PO_No, Invoice_No, 'Termed Licence Renewal' AS Category FROM LMS_Termed_Licence_Renewal
	UNION
    SELECT PO_No, Invoice_No, 'AI Licence Renewal' AS Category FROM LMS_AI_Licence_Renewal
    UNION
    SELECT PO_No, Invoice_No, 'CZL Account Setup' AS Category FROM CZL_Account_Setup_Charge
) TBL
GO
/****** Object:  View [dbo].[R_CZL_Licenced_Device]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_CZL_Licenced_Device]
AS
	SELECT A.Unique_ID
		 , A.Device_Serial
		 , A.Device_ID
		 , A.Model
		 , A.AI_Software_Version
		 , A.R_Version
		 , A.Scale_SN
		 , A.MAC_Addr
		 , A.Production_Licence_No
		 , A.Location
		 , A.Created_Date
		 , A.Last_Updated
		 , A.Effective_Date
		 , dbo.Get_Licence_Status(A.Device_ID) AS [Status]
		 , dbo.Get_Licence_Activated_Date(A.Device_ID) AS [Activated_Date]
		 , CASE WHEN dbo.Get_Licence_Status(A.Device_ID) != 'Blocked' THEN dbo.Get_Licence_Expiry_Date(A.Device_ID) ELSE '' END AS [Expiry_Date]
		 , A.CZL_Account_Unique_ID
		 , A.Client_ID AS [Account_ID]
		 , B.User_Group AS [Account_Name]
		 , B.By_Distributor AS [Distributor_Code]
		 , ISNULL(C.Name, '') AS Distributor
		 , B.Country
	FROM CZL_Licenced_Devices A
	INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID
	LEFT JOIN Master_Customer C ON C.Customer_ID = B.By_Distributor
GO
/****** Object:  View [dbo].[_Server_Space_Week]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Week]
AS
	SELECT YEAR(Week_Start) AS [Year]
	     , DATEPART(ISO_WEEK, Week_Start) AS [Week]
		 , Week_Start AS [Week Start]
		 , Week_End AS [Week End]
		 , MAX(Size) AS [Server Space]
		 , MAX(Used) AS [Used]
		 , MAX(Avail) AS [Available]
		 , MAX(Used_Percentage) AS [Usage]
		 , MAX(DB_Size) AS [DB Size]
	FROM (
			SELECT DATEADD(DAY, 2 - DATEPART(WEEKDAY, CAST(Reading_Date AS date)), CAST(Reading_Date AS date)) AS Week_Start
				 , DATEADD(DAY, -1, DATEADD(WEEK, 1, DATEADD(DAY, 2 - DATEPART(WEEKDAY, CAST(Reading_Date AS date)), CAST(Reading_Date AS date)))) AS Week_End
				 , Size
				 , Used
				 , Avail
				 , Used_Percentage
				 , DB_Size
			FROM Server_Space
		 ) TBL
	WHERE DB_Size > 0
	GROUP BY YEAR(Week_Start), DATEPART(ISO_WEEK, Week_Start), Week_Start, Week_End
GO
/****** Object:  View [dbo].[_Server_Space_Week_Growth]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Week_Growth]
AS
	WITH CTE AS (
					SELECT ROW_NUMBER() OVER (ORDER BY A.[Year], A.[Week]) AS [Row No]
					     , A.[Year], A.[Week] AS [Week No]
						 , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 --, ISNULL(CAST((MAX(A.[Used]) - MAX(B.[Used])) / MAX(B.[Used]) * 100 AS decimal(10, 2)), 0) AS [Used Growth]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[Available]) AS [Available]
						 , ISNULL(MAX(A.[Available]) - MAX(B.[Available]), 0) AS [Avail Diff]
						 , MAX(A.[Usage]) AS [Usage]
						 , ISNULL(MAX(A.[Usage]) - MAX(B.[Usage]), 0) AS [Usage Diff]
						 , MAX(A.[DB Size]) AS [DB Size]
						 --, ISNULL(CAST((MAX(A.[DB Size]) - MAX(B.[DB Size])) / MAX(B.[DB Size]) * 100 AS decimal(10, 2)), 0) AS [DB Growth]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Week A
					LEFT JOIN _Server_Space_Week B ON B.[Week End] = DATEADD(DAY, -1, A.[Week Start])
					GROUP BY A.[Year], A.[Week], CAST(A.[Week Start] AS nvarchar)
	)

SELECT [Row No], [Year], [Week No]
     , [Server Space]
	 , [Used], [Used Growth]
	 , [Available], [Avail Diff]
	 , [Usage], [Usage Diff]
	 , [DB Size], [DB Growth]
FROM CTE;

GO
/****** Object:  View [dbo].[_Server_Space_Quarter_Growth]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Quarter_Growth]
AS
	WITH CTE AS (
					SELECT ROW_NUMBER() OVER (ORDER BY A.[Year], A.[Quarter No]) AS [Row No]
					     , A.[Year], A.[Quarter No] AS [Quarter]
						 , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 --, ISNULL(CAST((MAX(A.[Used]) - MAX(B.[Used])) / MAX(B.[Used]) * 100 AS decimal(10, 2)), 0) AS [Used Growth]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[Available]) AS [Available]
						 , ISNULL(MAX(A.[Available]) - MAX(B.[Available]), 0) AS [Avail Diff]
						 , MAX(A.[Usage]) AS [Usage]
						 , ISNULL(MAX(A.[Usage]) - MAX(B.[Usage]), 0) AS [Usage Diff]
						 , MAX(A.[DB Size]) AS [DB Size]
						 --, ISNULL(CAST((MAX(A.[DB Size]) - MAX(B.[DB Size])) / MAX(B.[DB Size]) * 100 AS decimal(10, 2)), 0) AS [DB Growth]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Quarter A
					LEFT JOIN _Server_Space_Quarter B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Year], A.[Quarter No], CAST(A.[Start Date] AS nvarchar)
	) 

SELECT [Row No], [Year], [Quarter]
     , [Server Space]
	 , [Used], [Used Growth]
	 , [Available], [Avail Diff]
	 , [Usage], [Usage Diff]
	 , [DB Size], [DB Growth]
FROM CTE;

GO
/****** Object:  View [dbo].[_Server_Space_Month]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Month]
AS
	SELECT YEAR(Reading_Date) AS [Year]
		 , LEFT(DATENAME(Month, Reading_Date), 3) AS [Month]
	     , DATEFROMPARTS(YEAR(MIN(CAST(Reading_Date AS date))), MONTH(MIN(CAST(Reading_Date AS date))), 1) AS [Start Date]
		 , EOMONTH(MAX(CAST(Reading_Date AS date))) AS [End Date]
		 , MAX(Size) AS [Server Space]
		 , MAX(Used) AS [Used]
		 , MAX(Avail) AS [Available]
		 , MAX(Used_Percentage) AS [Usage]
		 , MAX(DB_Size) AS [DB Size]
	FROM Server_Space
	WHERE DB_Size > 0
	GROUP BY YEAR(Reading_Date), DATENAME(Month, Reading_Date)
GO
/****** Object:  View [dbo].[_Module_Licence_Key]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Module_Licence_Key]
AS
	SELECT L.Customer_ID AS [Customer ID]
		 , C.Name AS [Licensee]
		 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
		 , L.PO_No AS [PO No]
		 , L.PO_Date AS [PO Date]
		 , L.Invoice_No AS [Invoice No]
		 , L.Invoice_Date AS [Invoice Date]
		 , L.Application_Type AS [Application Type]
		 , L.OS_Type AS [OS Type]
		 , L.Licence_Code AS [Licence Code]
		 , L.Serial_No AS [Serial No]
		 , L.AI_Device_ID AS [AI Device ID]
		 , L.AI_Device_Serial_No AS [AI Device Serial No]
		 , L.Synced_dmcmobiletoken_term AS [Licence Term]
		 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
		 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
		 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
		 , L.Created_Date AS [Created Date]
		 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
		 , CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
		        THEN 'No Expiry' 
				ELSE CONVERT(nvarchar, DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 23) 
				END AS [Expired Date]
		 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		        THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				          THEN L.Synced_dmcmobiletoken_status
						  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() AND L.Synced_dmcmobiletoken_status != 'Renew' THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
						  END
				ELSE L.Synced_dmcmobiletoken_status
				END AS [Status]
		 , L.Licensee_Email AS [Email]
		 , SR.Sales_Representative_ID AS [Requestor ID]
		 , SR.Name AS [Requested By]
		 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
		 , L.Remarks 
	FROM LMS_Licence AS L 
	INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
	INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
	WHERE L.Application_Type IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence') 
GO
/****** Object:  Table [dbo].[LMS_Module_Licence_Activated]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Module_Licence_Activated](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Licence_Code] [nvarchar](100) NULL,
	[Activated_Module_Type] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Activated_AI_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Activated_AI_Licence]
AS

	WITH ActivatedAILicence AS (
		SELECT [Customer ID], [Licensee], [Application Type], [OS Type], [Chargeable], [Created Date]
			 , [Licence Code], [Status], [Serial No], [MAC Address], [AI Device ID], [AI Device Serial No], [Activated Date], [Licence Term]
			 , CASE WHEN [Expired Date] = 'No Expiry' THEN 'No Expiry' 
					ELSE CAST([Expired Date] AS nvarchar) 
				    END AS [Expired Date]
			 , [Remarks], [Requestor ID], [Requested By]
			 , ROW_NUMBER() OVER (PARTITION BY [Customer ID], [AI Device ID], [MAC Address] ORDER BY [Activated Date] DESC) AS RowNum
		FROM (
				SELECT [Customer ID], [Licensee]
					 , ISNULL([Application Type] + ' (' + LMS_Module_Licence_Activated.Activated_Module_Type + ') ', [Application Type]) AS [Application Type]
					 , [OS Type], [Chargeable], [Created Date], [Licence Code], [Status], [Serial No]
					 , [MAC Address], [AI Device ID], [AI Device Serial No]
					 , [Activated Date], [Licence Term], [Expired Date], [Remarks], [Requestor ID], [Requested By]
				FROM _Module_Licence_Key
				LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(_Module_Licence_Key.[Licence Code], '-', '')
				WHERE [Status] NOT IN ('Blocked', 'New') 
				  AND (LEN([AI Device ID]) > 0 OR ISNULL([Application Type] + ' (' + LMS_Module_Licence_Activated.Activated_Module_Type + ') ', [Application Type]) LIKE '%AI%')		
		     ) TBL
	)

	SELECT [Customer ID], [Licensee], [Application Type]
	     , [OS Type], [Chargeable], [Created Date]
		 , [Licence Code] 
		 , [Status]
		 , [Serial No], [MAC Address], [AI Device ID], [AI Device Serial No], [Activated Date], [Licence Term], [Expired Date]
		 , [Remarks], [Requestor ID], [Requested By]
	FROM ActivatedAILicence
	WHERE RowNum = 1;

GO
/****** Object:  View [dbo].[R_CZL_Licenced_Device_With_Unassigned_Device]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[R_CZL_Licenced_Device_With_Unassigned_Device]
AS
	SELECT A.Unique_ID
	     , ISNULL(dbo.Get_AI_Licence_Activation_Key(A.Device_ID), 'No Binding Key') AS Licence_Key
		 , A.Device_Serial
		 , A.Device_ID
		 , A.Model
		 , A.AI_Software_Version
		 , A.R_Version
		 , A.Scale_SN
		 , A.MAC_Addr
		 , A.Production_Licence_No
		 , A.Location
		 , A.Created_Date
		 , A.Last_Updated
		 , A.Effective_Date
		 , dbo.Get_Licence_Status(A.Device_ID) AS [Status]
		 , dbo.Get_Licence_Activated_Date(A.Device_ID) AS [Activated_Date]
         , CASE WHEN dbo.Get_AI_Licence_Activation_Key(A.Device_ID) IS NOT NULL 
		        THEN CASE WHEN dbo.Get_AI_Licence_Term(A.Device_ID) < 9999 
                          THEN CAST(dbo.Get_AI_Licence_Term(A.Device_ID) / 12 AS varchar(20)) + ' Year(s)'
                          ELSE 'No Expiry' END
			    ELSE NULL
                END AS [Licence_Term]
		 , CASE WHEN dbo.Get_Licence_Status(A.Device_ID) != 'Blocked' THEN dbo.Get_Licence_Expiry_Date(A.Device_ID) ELSE '' END AS [Expiry_Date]
		 , A.CZL_Account_Unique_ID
		 , A.Client_ID AS [Account_ID]
		 , B.User_Group AS [Account_Name]
		 , B.By_Distributor AS [Distributor_Code]
		 , ISNULL(C.Name, '') AS Distributor
		 , B.Country
	FROM CZL_Licenced_Devices A
	INNER JOIN CZL_Account B ON B.CZL_Account_Unique_ID = A.CZL_Account_Unique_ID
	LEFT JOIN Master_Customer C ON C.Customer_ID = B.By_Distributor
	UNION ALL
    SELECT NULL AS [Unique ID], [Licence Code], [AI Device Serial No], [AI Device ID], NULL AS [Model], NULL AS [AI Software Version], NULL AS [R Version], [Serial No]
	     , [MAC Address], NULL AS [Production Licence No], NULL AS [Location], [Created Date], [Activated Date] AS [Last Updated], NULL AS [Effective Date]  
		 , dbo.Get_Licence_Status(R_Activated_AI_Licence.[AI Device ID]) AS [Status]
         , [Activated Date]
		 , CASE WHEN [Licence Term] < 9999
		        THEN CAST([Licence Term] / 12 AS varchar(20)) + ' Year(s)'
				ELSE 'No Expiry'
				END AS [Licence_Term]
		 , CASE WHEN dbo.Get_Licence_Status(R_Activated_AI_Licence.[AI Device ID]) != 'Blocked' THEN dbo.Get_Licence_Expiry_Date(R_Activated_AI_Licence.[AI Device ID]) ELSE '' END AS [Expiry_Date]
		 , NULL AS [Account Unique ID], '' AS [Account ID], 'Unassigned' AS [Account Name]
		 , [Customer ID] AS [Distributor Code], Master_Customer.Name AS [Distributor]
		 , (SELECT Country FROM Master_Customer WHERE Customer_ID = [Customer ID]) AS [Country]
	FROM R_Activated_AI_Licence
    LEFT JOIN Master_Customer ON Customer_ID = R_Activated_AI_Licence.[Customer ID]
    WHERE [AI Device ID] NOT IN (SELECT Device_ID FROM CZL_Licenced_Devices)
	  AND LEN([Serial No]) > 0
GO
/****** Object:  View [dbo].[_Server_Space_Month_Growth]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Month_Growth]
AS
	WITH CTE AS (
					SELECT ROW_NUMBER() OVER (ORDER BY A.[Year], A.[Month]) AS [Row No]
					     , A.[Year], A.[Month]
					     , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[Available]) AS [Available]
						 , ISNULL(MAX(A.[Available]) - MAX(B.[Available]), 0) AS [Avail Diff]
						 , MAX(A.[Usage]) AS [Usage]
						 , ISNULL(MAX(A.[Usage]) - MAX(B.[Usage]), 0) AS [Usage Diff]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Month A
					LEFT JOIN _Server_Space_Month B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Year], A.[Month], CAST(A.[Start Date] AS nvarchar)
	) 

SELECT [Row No], [Year], [Month]
     , [Server Space]
	 , [Used], [Used Growth]
	 , [Available], [Avail Diff]
	 , [Usage], [Usage Diff]
	 , [DB Size], [DB Growth]
FROM CTE;

GO
/****** Object:  View [dbo].[R_Hardkey_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Hardkey_Licence]
AS
	SELECT HL.Customer_ID
	     , C.Name AS [Customer]
		 , C.Country AS [Country]
		 , HL.PO_No AS [PO No]
		 , HL.PO_Date AS [PO Date]
		 , HL.SO_No AS [SO No]
		 , HL.SO_Date AS [SO Date]
		 , HL.Invoice_No AS [Invoice No]
		 , HL.Invoice_Date AS [Invoice Date]
		 , HL.Licence_No AS [Licence No]
		 , HL.PLU_Code AS [PLU Code]
		 , (SELECT Top 1 Value_2 FROM DB_Lookup WHERE Value_1 = HL.PLU_Code) AS [Description]
		 , HL.[Prepared_By] AS [Prepared By]
		 , HL.Created_Date AS [Created Date]
		 , HL.Start_Date AS [Start Date]
		 , HL.End_Date AS [End Date]
		 , (SELECT Top 1 Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = HL.Sales_Representative_ID) AS [Requested By]
	FROM LMS_Hardkey_Licence AS HL 
	INNER JOIN Master_Customer AS C ON C.Customer_ID = HL.Customer_ID
GO
/****** Object:  View [dbo].[D_LMS_Hardkey_Licence_Order_Outstanding_Invoice]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_LMS_Hardkey_Licence_Order_Outstanding_Invoice]
AS
	SELECT Customer_ID AS [Customer ID]
		 , Customer AS [Licensee]
		 , [PO No]
		 , [PO Date]
		 , [SO No]
		 , [SO Date]
		 , [Licence No]
		 , [Created Date]
		 , [PLU Code]
		 , [Description]
		 , [Prepared By]
		 , [Requested By]
		 , [Invoice No]
		 , [Invoice Date]
	FROM R_Hardkey_Licence 
	WHERE [Invoice No] = ''
GO
/****** Object:  View [dbo].[D_CZL_Account_Setup_Fee_Outstanding_Invoice]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[D_CZL_Account_Setup_Fee_Outstanding_Invoice]
AS
	SELECT [By Distributor]
		 , [Country]
		 , [CZL Account ID]
		 , Client_ID AS [Account ID]
		 , [User Group] AS [Account Name]
		 , [Created Date]
		 , PO_No AS [PO No]
		 , PO_Date AS [PO Date]
		 , [Currency]
		 , Fee AS [Amount]
		 , Name AS [Requested By]
		 , Invoice_No AS [Invoice No]
		 , Invoice_Date AS [Invoice Date]
	FROM CZL_Account_Setup_Charge A
	INNER JOIN R_CZL_Account B ON B.[CZL Account ID] = A.CZL_Account_Unique_ID AND B.[CZL Client ID] = A.Client_ID
	INNER JOIN Master_Sales_Representative S ON S.Sales_Representative_ID = A.Sales_Representative_ID
	WHERE A.Invoice_No = ''
GO
/****** Object:  Table [dbo].[FTP_Server_Distributor_Account]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FTP_Server_Distributor_Account](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[User_Group] [nvarchar](30) NOT NULL,
	[Contact_Person] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[User_ID] [nvarchar](10) NULL,
	[User_Password] [nvarchar](10) NULL,
	[Is_Active] [bit] NULL,
	[Last_Update] [date] NULL,
	[Code] [nvarchar](4) NULL,
	[Access_List_UID] [nvarchar](20) NULL,
	[Remarks] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_FTP_Users_Creation_Command]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_FTP_Users_Creation_Command]
AS
SELECT User_ID AS [FTP User], User_Password AS [FTP Password], Contact_Person AS [Full Name], User_Group AS [Role]
     , 'net user ' + User_ID + ' ' + User_Password + ' /add /fullname:"' + Contact_Person + '" /passwordchg:no /expires:never /comment:"' + User_Group + '"'
     + ' && ' + 'wmic useraccount where name=''' + User_ID + ''' set PasswordExpires=false' 
	 + ' && ' + 'net localgroup "SFTP Users" ' + User_ID + ' /add' AS [User Creation Command]
	 , CASE WHEN Is_Active = 1 THEN 'Active' ELSE 'In-active' END AS [Account Status] 
	 , 'net user ' + User_ID + ' /active:' + CASE WHEN Is_Active = 1 THEN 'no' ELSE 'yes' END AS [Enabe Disable Account Command]
FROM FTP_Server_Distributor_Account 

GO
/****** Object:  View [dbo].[R_LMS_Module_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_LMS_Module_Licence]
AS
SELECT L.Customer_ID AS [Customer ID]
     , C.Name AS [Licensee]
	 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
     , L.PO_No AS [PO No]
	 , L.PO_Date AS [PO Date]
	 , CASE WHEN Is_Cancelled = 1 THEN 'CANCELLED' ELSE L.Invoice_No END AS [Invoice No]
	 , L.Invoice_Date AS [Invoice Date]
	 , L.Application_Type AS [Application Type]
	 , L.OS_Type AS [OS Type]
	 , L.Licence_Code AS [Licence Code]
	 , L.Synced_dmcmobiletoken_term AS [Licence Term]
	 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
	 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
	 , L.Serial_No AS [Serial No]
	 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
	 , L.AI_Device_ID AS [AI Device ID]
	 , L.AI_Device_Serial_No AS [AI Device Serial No]
	 , L.Created_Date AS [Created Date]
	 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
	        THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
	                  THEN 'No Expiry' 
			          ELSE FORMAT(DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 'dd MMM yy') END
			ELSE CASE WHEN DATEDIFF(YEAR, GETDATE(), L.Synced_dmcmobiletoken_expireddate) > 50
			          THEN ''
					  ELSE FORMAT(DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 'dd MMM yy')
					  END
			END AS [Expired Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		    THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				      THEN L.Synced_dmcmobiletoken_status
					  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() AND L.Synced_dmcmobiletoken_status != 'Renew' THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
					  END
		    ELSE L.Synced_dmcmobiletoken_status
		    END AS [Status]
	 , L.Licensee_Email AS [Email]
	 , CASE WHEN SR.Sales_Representative_ID = 'S0032' AND L.Customer_ID = 'CTR-000046' THEN 'S0010' ELSE SR.Sales_Representative_ID END AS [Requestor ID]
	 , CASE WHEN SR.Sales_Representative_ID = 'S0032' AND L.Customer_ID = 'CTR-000046' THEN (SELECT TOP 1 Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = 'S0010') ELSE SR.Name END AS [Requested By]
	 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
	 , L.Remarks 
FROM LMS_Licence AS L 
INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
WHERE L.Application_Type IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence')
GO
/****** Object:  View [dbo].[_AllRegisteredAILicence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_AllRegisteredAILicence]
AS
	SELECT [Customer ID]
		 , [Licensee]
		 , ISNULL([Application Type] + ' (' + LMS_Module_Licence_Activated.Activated_Module_Type + ') ', [Application Type]) AS [Application Type]
		 , [OS Type]
		 , [Chargeable]
		 , [Created Date]
		 , [Licence Code]
		 , [Status]
		 , [Serial No]
		 , [MAC Address]
		 , [AI Device ID]
		 , [AI Device Serial No]
		 , [Activated Date]
		 , [Expired Date]
		 , [Remarks]
		 , [Requested By]
	FROM _Module_Licence_Key
	LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(_Module_Licence_Key.[Licence Code], '-', '')
	WHERE (LEN([AI Device ID]) > 0 OR ISNULL([Application Type] + ' (' + LMS_Module_Licence_Activated.Activated_Module_Type + ') ', [Application Type]) LIKE '%AI%')
GO
/****** Object:  View [dbo].[R_Maintenance_Customer]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Maintenance_Customer]
AS
	SELECT Customer_ID AS [Customer ID]
	     , Name AS [Name]
		 , Address AS [Address]
		 , Created_Date AS [Created Date]
		 , CASE WHEN Is_Active = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
		 , Last_Updated AS [Last Updated]
		 , Services_Group AS [Services Group]
		 , Contact_Person AS [Contact Person]
		 , Phone AS [Phone]
		 , Email AS [Email]
	FROM Maintenance_Customer
GO
/****** Object:  View [dbo].[_AllRegisteredAILicencePivot]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_AllRegisteredAILicencePivot]
AS
	SELECT [Customer ID]
		 , [Licensee]
		 , ISNULL([Activated], 0) AS [Activated]
		 , ISNULL([Renew], 0) AS [Renew]
		 , ISNULL([Expired], 0) AS [Expired]
		 , ISNULL([Blocked], 0) AS [Blocked]
		 , (ISNULL([Renew], 0) + ISNULL([Activated], 0) + ISNULL([Expired], 0) + ISNULL([Blocked], 0)) AS [Total]
	FROM (
		SELECT [Customer ID], [Licensee], [Status]
		FROM _AllRegisteredAILicence
	) AS SourceTable
	PIVOT (
		COUNT([Status])
		FOR [Status] IN ([Activated], [Renew], [Expired], [Blocked])
	) AS PivotTable
GO
/****** Object:  View [dbo].[R_LMS_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_LMS_Licence]
AS
SELECT L.Customer_ID AS [Customer ID]
     , C.Name AS [Licensee]
	 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
     , L.PO_No AS [PO No]
	 , L.PO_Date AS [PO Date]
	 , CASE WHEN Is_Cancelled = 1 THEN 'CANCELLED' ELSE L.Invoice_No END AS [Invoice No]
	 , L.Invoice_Date AS [Invoice Date]
	 , (SELECT Value_2 FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_4 IN ('App Licence', 'DMC Server Licence Key') AND Value_3 = L.Application_Type) AS [Application Type]
	 , L.OS_Type AS [OS Type]
	 , L.Licence_Code AS [Licence Code]
	 , L.Synced_dmcmobiletoken_term AS [Licence Term]
	 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
	 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
	 , L.Serial_No AS [Serial No]
	 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
	 , L.AI_Device_ID AS [AI Device ID]
	 , L.AI_Device_Serial_No AS [AI Device Serial No]
	 , L.Created_Date AS [Created Date]
	 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
	        THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
	                  THEN 'No Expiry' 
			          ELSE FORMAT(DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 'dd MMM yy') END
			ELSE CASE WHEN DATEDIFF(YEAR, GETDATE(), L.Synced_dmcmobiletoken_expireddate) > 50
			          THEN ''
					  ELSE FORMAT(DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 'dd MMM yy')
					  END
			END AS [Expired Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		    THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				      THEN L.Synced_dmcmobiletoken_status
					  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() AND L.Synced_dmcmobiletoken_status != 'Renew' THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
					  END
		    ELSE L.Synced_dmcmobiletoken_status
		    END AS [Status]
	 , L.Licensee_Email AS [Email]
	 , SR.Sales_Representative_ID AS [Requestor ID]
	 , SR.Name AS [Requested By]
	 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
	 , L.Remarks 
	 , Is_Cancelled AS [Is Cancelled]
FROM LMS_Licence AS L 
INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
WHERE L.Application_Type NOT IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence')
GO
/****** Object:  View [dbo].[_LicenceUsage]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_LicenceUsage]
AS
	SELECT [Customer ID], [Licence Type], ISNULL(New, 0) AS New, ISNULL(Used, 0) AS Used 
	FROM (
			SELECT [Customer ID], [Application Type] AS [Licence Type], 'New' AS [Status], COUNT([Licence Code]) AS Quantity
			FROM R_LMS_Licence
			WHERE [Status] IN ('New') 
			GROUP BY [Customer ID], [Application Type]
			UNION ALL
			SELECT [Customer ID], [Application Type], 'Used' AS [Status], COUNT([Licence Code]) AS Quantity
			FROM R_LMS_Licence
			WHERE [Status] NOT IN ('New') 
			GROUP BY [Customer ID], [Application Type]
		 ) TBL
	PIVOT
	(   SUM(Quantity)
		For Status IN ([New], [Used])
	) AS PVT
GO
/****** Object:  View [dbo].[_App_Product_Licence_Key]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_App_Product_Licence_Key]
AS
	SELECT L.Customer_ID AS [Customer ID]
		 , C.Name AS [Licensee]
		 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
		 , L.PO_No AS [PO No]
		 , L.PO_Date AS [PO Date]
		 , L.Invoice_No AS [Invoice No]
		 , L.Invoice_Date AS [Invoice Date]
		 , L.Application_Type AS [Application Type]
		 , L.OS_Type AS [OS Type]
		 , L.Licence_Code AS [Licence Code]
		 , L.Serial_No AS [Serial No]
		 , L.AI_Device_ID AS [AI Device ID]
		 , L.AI_Device_Serial_No AS [AI Device Serial No]
		 , L.Synced_dmcmobiletoken_term AS [Licence Term]
		 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
		 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
		 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
		 , L.Created_Date AS [Created Date]
		 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
		 , CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
		        THEN 'No Expiry' 
				ELSE CONVERT(nvarchar, DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 23) 
				END AS [Expired Date]
		 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		        THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				          THEN L.Synced_dmcmobiletoken_status
						  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
						  END
				ELSE L.Synced_dmcmobiletoken_status
				END AS [Status]
		 , L.Licensee_Email AS [Email]
		 , SR.Name AS [Requested By]
		 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
		 , L.Remarks 
	FROM LMS_Licence AS L 
	INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
	INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
	WHERE L.Application_Type NOT IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence') 
GO
/****** Object:  Table [dbo].[DB_Recovered_Invoice]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Recovered_Invoice](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Invoice_No] [nvarchar](30) NOT NULL,
	[Invoice_Date] [date] NULL,
	[Item_Code] [nvarchar](50) NULL,
	[Currency] [nvarchar](5) NULL,
	[Amount] [money] NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[PO_No] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[I_DB_Recovered_Invoice]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_DB_Recovered_Invoice]
AS
	SELECT Invoice_No, Invoice_Date
	     , Item_Code
	     , (SELECT Value_2 FROM DB_Lookup WHERE Value_1 = Item_Code) AS Description
		 , Currency, SUM(ISNULL(Amount, 0)) AS Amount
		 , TBL.Customer_ID
		 , C.Name AS Customer
		 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS Invoice_Bill_To
	FROM (
			SELECT '' AS Subscription_ID  --DISTINCT Subscription_ID
				 , Invoice_No
				 , Invoice_Date
				 , Item_Code
				 , DB_Recovered_Invoice.Currency
				 , Amount
				 , Customer_ID
			FROM DB_Recovered_Invoice
			--LEFT JOIN DMC_Subscription ON DMC_Subscription.Ref_Invoice_No = DB_Recovered_Invoice.Invoice_No
			UNION ALL
			SELECT Subscription_ID
				 , Ref_Invoice_No
				 , Invoiced_Date
				 , CASE WHEN dbo.Get_Subscriber_Group(Subscription_ID) = 'H' 
						THEN CASE WHEN dbo.Get_NumberOfDays(Start_Date, End_Date) >= 364 THEN (SELECT Value_1 FROM DB_Lookup WHERE Value_2 = 'DMC Annual Subscription Fee') ELSE (SELECT Value_1 FROM DB_Lookup WHERE Value_2 = 'DMC Monthly Subscription Fee') END
						ELSE CASE WHEN dbo.Get_NumberOfDays(Start_Date, End_Date) >= 364 THEN (SELECT Value_1 FROM DB_Lookup WHERE Value_2 = 'DMC Annual Subscription Fee (Retail)') ELSE (SELECT Value_1 FROM DB_Lookup WHERE Value_2 = 'DMC Monthly Subscription Fee') END
						END AS Item_Code
				 , Currency
				 , ROUND(SUM(Fee), 2) AS Amount
				 , DMC_Headquarter.Customer_ID
			FROM DMC_Subscription
			LEFT JOIN DMC_Headquarter ON DMC_Headquarter.Headquarter_ID = SUBSTRING(Store_ID, 2, 6)
			GROUP BY Subscription_ID, Ref_Invoice_No, Invoiced_Date, Start_Date, End_Date, Currency, Customer_ID
	) TBL
	INNER JOIN Master_Customer C ON C.Customer_ID = TBL.Customer_ID
	WHERE Invoice_No IS NOT NULL
	GROUP BY Invoice_No, Invoice_Date, Item_Code, Currency, TBL.Customer_ID, C.Name, C.By_Distributor
GO
/****** Object:  Table [dbo].[LMS_Module_Licence_Order_Item]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Module_Licence_Order_Item](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UID] [nvarchar](20) NULL,
	[Module_Type] [nvarchar](20) NULL,
	[Quantity] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[_AILicenceOfAllStatusAndTerm]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_AILicenceOfAllStatusAndTerm]
AS
	WITH PO_Of_AI_Licence AS (
		SELECT Customer_ID, PO_No, Module_Type, Created_Date
		FROM LMS_Module_Licence_Order
		INNER JOIN LMS_Module_Licence_Order_Item 
			ON LMS_Module_Licence_Order_Item.UID = LMS_Module_Licence_Order.UID
		WHERE Module_Type IN ('AI')
	)

	SELECT [Customer ID], [Licensee]
		 , [PO No]
		 , CASE WHEN [Status] = 'New' 
				THEN [Application Type]
				ELSE [Application Type] + ' (' + LMS_Module_Licence_Activated.Activated_Module_Type + ')'
				END AS [Application Type]
		, [Licence Code]
		, [Serial No]
		, [AI Device ID]
		, [AI Device Serial No]
		, [MAC Address]
		, CASE WHEN [Licence Term] >= 9999 THEN 'Non-Expiry'
			   WHEN [Licence Term] >= 12 THEN CAST([Licence Term] / 12 AS varchar(10)) + ' Year(s)'
			   ELSE CAST([Licence Term] AS varchar(10)) + ' Month(s)'
			   END AS [Licence Term]
		, [Status]
		, [Requested By]
		, [Chargeable]
	FROM _Module_Licence_Key 
	INNER JOIN PO_Of_AI_Licence ON PO_Of_AI_Licence.Customer_ID = _Module_Licence_Key.[Customer ID] 
							   AND PO_Of_AI_Licence.PO_No = _Module_Licence_Key.[PO No]
							   AND PO_Of_AI_Licence.Created_Date = _Module_Licence_Key.[Created Date]
	LEFT JOIN LMS_Module_Licence_Activated 
			ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(_Module_Licence_Key.[Licence Code], '-', '')
	WHERE [Status] NOT IN ('Blocked')
		AND [Customer ID] NOT IN ('CTR-000005', 'CTR-000081', 'CTR-000121')
		-- Only include rows where either:
		--   a) Status is 'New' (which returns "PC Scale")
		--   OR
		--   b) For non-new statuses the activated module type contains "AI"
		AND ([Status] = 'New' OR LMS_Module_Licence_Activated.Activated_Module_Type LIKE '%AI%')
GO
/****** Object:  View [dbo].[R_Activated_Termed_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Activated_Termed_Licence]
AS
SELECT [Customer ID]
     , [Licensee]
	 , [Application Type]
	 , [OS Type]
	 , [Chargeable]
	 , [Created Date]
	 , [Licence Code]
	 , [Status]
	 , [Serial No]
	 , [MAC Address]
	 , [AI Device ID]
	 , [AI Device Serial No]
	 , [Activated Date]
	 , [Expired Date]
	 , [Remarks]
	 , [Requested By]
FROM _App_Product_Licence_Key
WHERE [Status] NOT IN ('Blocked') AND [Expired Date] NOT IN ('No Expiry')
GO
/****** Object:  Table [dbo].[LMS_Module_Licence_Pool]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Module_Licence_Pool](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Headquarter_Name] [nvarchar](100) NULL,
	[Synced_dmcstore_storeid] [nvarchar](5) NULL,
	[Store_No] [nvarchar](5) NULL,
	[Store_Name] [nvarchar](100) NULL,
	[Module_Type] [nvarchar](20) NULL,
	[Balance] [int] NULL,
	[Used] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_LMS_Module_Licence_Pool]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_LMS_Module_Licence_Pool]
AS
	SELECT CASE WHEN K.Value_1 IS NOT NULL THEN K.Value_1 ELSE H.Customer_ID END AS Customer_ID
		 , Synced_dmcstore_storeid
		 , P.Store_No AS No
		 , P.Store_Name AS Name
		 , L.Value_1 AS Module_Type
		 , P.Balance
		 , P.Used
		 --, *
	FROM LMS_Module_Licence_Pool P
	INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = FORMAT(CAST(P.Headquarter_ID AS int), 'd6')
	LEFT JOIN DB_Lookup K ON P.Synced_dmcstore_storeid = K.Value_3 AND K.Lookup_Name IN ('Module Licensee Bind')
	LEFT JOIN DB_Lookup L ON L.Value_2 = P.Module_Type AND L.Lookup_Name IN ('Module Term Mapping')
GO
/****** Object:  View [dbo].[_Module_Licence_Pool_Not_Tally]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Module_Licence_Pool_Not_Tally]
AS
	SELECT TBL1.Customer_ID AS [Customer ID]
		 , C.name AS [Licensee]
		 , tbl1.Module_Type AS [Module Type]
		 , Record_Quantity AS [LMS Portal Quantity]
		 , Live_Quantity AS [Live Quantity]
	FROM (
			SELECT Customer_ID
				 , Module_Type
				 , SUM(Quantity) AS Record_Quantity
			FROM LMS_Module_Licence_Order A
			INNER JOIN LMS_Module_Licence_Order_Item B ON B.UID = A.UID
			WHERE Is_Cancelled = 0
			GROUP BY A.Customer_ID, B.Module_Type
	) TBL1
	INNER JOIN (
		SELECT Customer_ID, Module_Type, SUM((Balance + Used)) AS Live_Quantity
		FROM R_LMS_Module_Licence_Pool
		GROUP BY Customer_ID, Module_Type
	) TBL2 ON TBL2.Customer_ID = TBL1.Customer_ID AND TBL2.Module_Type = TBL1.Module_Type
	INNER JOIN Master_Customer C ON C.Customer_ID = tbl1.Customer_ID
	WHERE Record_Quantity != Live_Quantity
GO
/****** Object:  View [dbo].[I_LMS_Module_Licence]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[I_LMS_Module_Licence]
AS
SELECT L.Customer_ID AS [Customer ID]
     , C.Name AS [Licensee]
	 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
     , L.PO_No AS [PO No]
	 , L.PO_Date AS [PO Date]
	 , L.Invoice_No AS [Invoice No]
	 , L.Invoice_Date AS [Invoice Date]
	 , L.Application_Type AS [Application Type]
	 , L.OS_Type AS [OS Type]
	 , L.Licence_Code AS [Licence Code]
	 , L.Synced_dmcmobiletoken_term AS [Licence Term]
	 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
	 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
	 , L.Serial_No AS [Serial No]
	 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
	 , L.Created_Date AS [Created Date]
	 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
	 , CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
	        THEN 'No Expiry' 
			ELSE CONVERT(nvarchar, DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 23) 
			END AS [Expired Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		    THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				      THEN L.Synced_dmcmobiletoken_status
					  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
					  END
		    ELSE L.Synced_dmcmobiletoken_status
		    END AS [Status]
	 , L.Licensee_Email AS [Email]
	 , SR.Name AS [Requested By]
	 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
	 , L.Remarks 
	 , L.Is_Cancelled AS [Is Cancelled]
FROM LMS_Licence AS L 
INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
WHERE L.Application_Type IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Module Licence') 
GO
/****** Object:  View [dbo].[_Server_Space_Semiannual_Growth]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Server_Space_Semiannual_Growth]
AS
	WITH CTE AS (
					SELECT ROW_NUMBER() OVER (ORDER BY A.[Year], A.[Semiannual]) AS [Row No]
					     , A.[Year], A.[Semiannual]
						 , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 --, ISNULL(CAST((MAX(A.[Used]) - MAX(B.[Used])) / MAX(B.[Used]) * 100 AS decimal(10, 2)), 0) AS [Used Growth]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[Available]) AS [Available]
						 , ISNULL(MAX(A.[Available]) - MAX(B.[Available]), 0) AS [Avail Diff]
						 , MAX(A.[Usage]) AS [Usage]
						 , ISNULL(MAX(A.[Usage]) - MAX(B.[Usage]), 0) AS [Usage Diff]
						 , MAX(A.[DB Size]) AS [DB Size]
						 --, ISNULL(CAST((MAX(A.[DB Size]) - MAX(B.[DB Size])) / MAX(B.[DB Size]) * 100 AS decimal(10, 2)), 0) AS [DB Growth]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Semiannual A
					LEFT JOIN _Server_Space_Semiannual B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Year], A.[Semiannual], CAST(A.[Start Date] AS nvarchar)
	)

SELECT [Row No], [Year], [Semiannual]
     , [Server Space]
	 , [Used], [Used Growth]
	 , [Available], [Avail Diff]
	 , [Usage], [Usage Diff]
	 , [DB Size], [DB Growth]
FROM CTE;

GO
/****** Object:  View [dbo].[_Server_Space_Year_Growth]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Server_Space_Year_Growth]
AS
	WITH CTE AS (
					SELECT ROW_NUMBER() OVER (ORDER BY A.[Year]) AS [Row No]
					     , A.[Year]
						 , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 --, ISNULL(CAST((MAX(A.[Used]) - MAX(B.[Used])) / MAX(B.[Used]) * 100 AS decimal(10, 2)), 0) AS [Used Growth]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[Available]) AS [Available]
						 , ISNULL(MAX(A.[Available]) - MAX(B.[Available]), 0) AS [Avail Diff]
						 , MAX(A.[Usage]) AS [Usage]
						 , ISNULL(MAX(A.[Usage]) - MAX(B.[Usage]), 0) AS [Usage Diff]
						 , MAX(A.[DB Size]) AS [DB Size]
						 --, ISNULL(CAST((MAX(A.[DB Size]) - MAX(B.[DB Size])) / MAX(B.[DB Size]) * 100 AS decimal(10, 2)), 0) AS [DB Growth]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Year A
					LEFT JOIN _Server_Space_Year B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Year], CAST(A.[Start Date] AS nvarchar)
	)

SELECT [Row No], [Year]
     , [Server Space]
	 , [Used], [Used Growth]
	 , [Available], [Avail Diff]
	 , [Usage], [Usage Diff]
	 , [DB Size], [DB Growth]
FROM CTE;

GO
/****** Object:  Table [dbo].[Headquarter_Device_Type]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Headquarter_Device_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Device_Type] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Store_Name_Change_History]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Store_Name_Change_History](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Store_ID] [nvarchar](20) NULL,
	[Old_Store_Name] [nvarchar](100) NULL,
	[New_Store_Name] [nvarchar](100) NULL,
	[Old_Banner_Name] [nvarchar](100) NULL,
	[New_Banner_Name] [nvarchar](100) NULL,
	[Effective_Date] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_DMC_Subscription_Detail]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_DMC_Subscription_Detail]
AS
SELECT A.Subscription_ID
     , H.Headquarter_ID
	 , H.Name AS Headquarter_Name
	 , A.Store_ID
	 , ISNULL(S.Synced_dmcstore_userstoreid, SUBSTRING(A.Store_ID, 8, 4)) AS Store_No
	 , CASE WHEN EXISTS(SELECT Old_Store_Name FROM DMC_Store_Name_Change_History WHERE Store_ID = A.Store_ID AND Effective_Date > A.Start_Date) 
	        THEN (SELECT TOP 1 Old_Store_Name FROM DMC_Store_Name_Change_History WHERE Store_ID = A.Store_ID AND Effective_Date > A.Start_Date ORDER BY Effective_Date)
			ELSE (SELECT Name FROM DMC_Store WHERE Store_ID = A.Store_ID)
			END AS Store_Name
	 , CASE WHEN EXISTS(SELECT Old_Banner_Name FROM DMC_Store_Name_Change_History WHERE Store_ID = A.Store_ID AND Effective_Date > A.Start_Date) 
	        THEN (SELECT Top 1 Old_Banner_Name FROM DMC_Store_Name_Change_History WHERE Store_ID = A.Store_ID AND Effective_Date > A.Start_Date ORDER BY Effective_Date)
			ELSE (SELECT Banner FROM DMC_Store WHERE Store_ID = A.Store_ID)
			END AS Banner
	 , S.Created_Date
	 , A.Start_Date
	 , A.End_Date
	 , A.Duration
	 , A.Currency
	 , A.Fee
     , CASE WHEN ( SELECT COUNT(*) FROM DMC_Subscription 
				   WHERE Store_ID = A.Store_ID AND Payment_Status NOT IN ('Cancelled')
		             AND Start_Date < ( SELECT TOP 1 Start_Date FROM DMC_Subscription 
										WHERE Subscription_ID = A.Subscription_ID ORDER BY Start_Date)
				 ) > 0 THEN 'Subscription Renewal' ELSE 'New Subscription' END Type
	 , H.Customer_ID
	 , C.Name AS Customer
	 , Trim(C.Country) AS Country
	 , CAST(HD.Device_Type AS nvarchar) AS Device_Type
FROM DMC_Subscription A 
INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = SUBSTRING(A.Store_ID, 2, 6)
LEFT JOIN Headquarter_Device_Type HD ON HD.Headquarter_ID = H.Headquarter_ID
LEFT JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
LEFT JOIN DMC_Store S ON S.Store_ID = A.Store_ID
WHERE A.Payment_Status NOT IN ('Cancelled')
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Monthly_Subscription](@ReportMonth date) RETURNS TABLE 
AS
RETURN 
(
	-- @ReportMonth return end date of previous month
	SELECT Subscription_ID
		 , Customer_ID
		 , Customer
		 , Country
		 , Device_Type
	     , Headquarter_ID
	     , Headquarter_Name
	     , Store_No
	     , Store_Name
		 , Created_Date
	     , Start_Date
	     , End_Date
	     , dbo.Get_NumberOfMonth(Start_Date, End_Date) AS No_Of_Month
	     , CASE WHEN DATEDIFF(Month, @ReportMonth, End_Date) <= dbo.Get_NumberOfMonth(Start_Date, End_Date)
	            THEN CASE WHEN DATEDIFF(Month, @ReportMonth, End_Date) > 0 THEN DATEDIFF(Month, @ReportMonth, End_Date) ELSE 0 END 
			    ELSE dbo.Get_NumberOfMonth(Start_Date, End_Date) END AS Remaining_Cycle
	     , CASE WHEN Currency != 'SGD' THEN 'SGD' ELSE Currency END AS Currency
	     , CAST((dbo.Monthly_Avg_Exchange_Rate(@ReportMonth, Currency) * Fee) / dbo.Get_NumberOfMonth(Start_Date, End_Date) AS decimal(10, 2)) AS Monthly_Fee
    FROM R_DMC_Subscription_Detail
    WHERE Start_Date <= @ReportMonth AND End_Date >= @ReportMonth
)
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_By_Country]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_By_Country](@ReportMonth date) RETURNS TABLE 
AS
RETURN 
(
	SELECT Country
		 , Headquarter_ID
		 , CASE WHEN Headquarter_Name LIKE '%HQ' THEN Customer ELSE Headquarter_Name END AS Headquarter_Name
		 , COUNT(Store_Name) AS Owned_Store
		 , Currency
		 , SUM(Monthly_Fee) AS Total_Amount_Per_Month
	FROM dbo.DMC_Monthly_Subscription(@ReportMonth)
	GROUP BY Country, Headquarter_ID, Customer, Headquarter_Name, Currency
)
GO
/****** Object:  View [dbo].[R_Termed_Licence_Renewal]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Termed_Licence_Renewal]
AS
	SELECT Renewal_UID AS [UID]
	     , Licence_Code AS [Licence Code]
		 , dbo.Get_AI_Licence_Expiry_Date(Licence_Code) AS [Expiry Date]
		 , PO_No AS [PO No]
		 , PO_Date AS [PO Date]
		 , Invoice_No AS [Invoice No]
		 , Invoice_Date AS [Invoice Date]
		 , Renewal_Date AS [Renewal Date]
		 , Chargeable
		 , Currency
		 , Fee
		 , Remarks
		 , Customer_ID AS [Customer ID]
		 , (SELECT Name FROM Master_Customer WHERE Customer_ID = LMS_Termed_Licence_Renewal.Customer_ID) AS Customer
		 , Sales_Representative_ID AS [Requestor ID]
		 , (SELECT Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = LMS_Termed_Licence_Renewal.Sales_Representative_ID) AS [Requested By]
	FROM LMS_Termed_Licence_Renewal
GO
/****** Object:  View [dbo].[I_Termed_Licence_Renewal]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_Termed_Licence_Renewal]
AS
	SELECT [Customer ID]
		 , [Licensee]
		 , [Application Type]
		 , [Licence Code]
		 , [MAC Address]
		 , [Activated Date]
		 , [Expired Date]
		 , [Status]
		 , ISNULL((SELECT TOP 1 PO_No FROM LMS_Termed_Licence_Renewal WHERE Licence_Code = R_Activated_Termed_Licence.[Licence Code] ORDER BY Renewal_UID DESC), (SELECT TOP 1 [PO No] FROM R_LMS_Licence WHERE [Licence Code] = R_Activated_Termed_Licence.[Licence Code])) AS [PO No]
		 , ISNULL((SELECT TOP 1 PO_Date FROM LMS_Termed_Licence_Renewal WHERE Licence_Code = R_Activated_Termed_Licence.[Licence Code] ORDER BY Renewal_UID DESC), (SELECT TOP 1 [PO Date] FROM R_LMS_Licence WHERE [Licence Code] = R_Activated_Termed_Licence.[Licence Code])) AS [PO Date]
		 , ISNULL((SELECT TOP 1 [Requested By] FROM R_Termed_Licence_Renewal WHERE [Licence Code] = R_Activated_Termed_Licence.[Licence Code] ORDER BY [UID] DESC), (SELECT TOP 1 [Requested By] FROM R_LMS_Licence WHERE [Licence Code] = R_Activated_Termed_Licence.[Licence Code])) AS [Requested By]
	FROM R_Activated_Termed_Licence WHERE [Expired Date] NOT IN ('No Expiry')
GO
/****** Object:  View [dbo].[D_Licence_With_Term]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[D_Licence_With_Term]
AS
SELECT [Customer ID], [Licensee], [Invoice Bill To], [PO No], [PO Date], [Invoice No], [Invoice Date]
     , [Application Type], [OS Type], [Licence Code], [Licence Term], [Max HQ], [Max Store], [Serial No], [MAC Address]
	 , [AI Device ID], [AI Device Serial No]
	 , [Created Date], [Activated Date]
     , CAST([Expired Date] AS date) AS [Expired Date]
	 , [Status], [Email], [Requestor ID], [Requested By], [Chargeable], [Remarks] 
FROM R_LMS_Licence 
WHERE [Licence Term] < 9999 AND [Status] NOT IN ('Blocked')
UNION
SELECT [Customer ID], [Licensee], [Invoice Bill To], [PO No], [PO Date], [Invoice No], [Invoice Date]
     , ISNULL([Application Type] + ' (' + Activated_Module_Type + ') ', [Application Type]) AS [Application Type], [OS Type], [Licence Code], [Licence Term], [Max HQ], [Max Store], [Serial No], [MAC Address]
	 , [AI Device ID], [AI Device Serial No]
	 , [Created Date], [Activated Date]
     , CAST([Expired Date] AS date) AS [Expired Date]
	 , [Status], [Email], [Requestor ID], [Requested By], [Chargeable], [Remarks] 
FROM R_LMS_Module_Licence 
LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(R_LMS_Module_Licence.[Licence Code], '-', '')
WHERE [Licence Term] < 9999 AND [Status] NOT IN ('Blocked')
GO
/****** Object:  Table [dbo].[DMC_User]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](20) NOT NULL,
	[Password] [nvarchar](20) NOT NULL,
	[Email] [nvarchar](100) NULL,
	[Created_Date] [date] NULL,
	[Effective_Date] [date] NULL,
	[Is_Active] [bit] NULL,
	[Inactive_Date] [date] NULL,
	[Synced_dmcuser_devicetype] [int] NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[_HeadquarterDeviceType]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_HeadquarterDeviceType]
AS
	WITH CTE AS (
		SELECT FORMAT(CAST(dbo.Check_Number_Series(Username) AS int), 'd6') AS Headquarter_ID
			 , CASE WHEN dbo.Check_Number_Series(VariedDeviceType) != 'ALL' 
					THEN CASE dbo.Check_Number_Series(VariedDeviceType) WHEN 0 THEN 'ALL' WHEN 1 THEN 'POS' WHEN 2 THEN 'RETAIL' END 
					ELSE dbo.Check_Number_Series(VariedDeviceType) END AS Device_Type
		FROM (
				SELECT STRING_AGG(Substring(Username, 1, 6), ',') AS Username
					 , STRING_AGG(Synced_dmcuser_devicetype, ',') AS VariedDeviceType 
				FROM DMC_User 
				GROUP BY Headquarter_ID
		) TBL1
		WHERE Len(VariedDeviceType) > 1
		UNION
		SELECT Username
			 , CASE VariedDeviceType WHEN 0 THEN 'ALL' WHEN 1 THEN 'POS' WHEN 2 THEN 'RETAIL' END AS [Device Type] 
		FROM (
				SELECT STRING_AGG(Substring(Username, 1, 6), ',') AS Username
					 , STRING_AGG(Synced_dmcuser_devicetype, ',') AS VariedDeviceType 
				FROM DMC_User 
				GROUP BY Headquarter_ID
		) TBL1
		WHERE Len(VariedDeviceType) <= 1
	)

	SELECT * FROM CTE
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_By_Account_Type]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_By_Account_Type](@ReportMonth date) RETURNS TABLE 
AS
RETURN 
(
	SELECT Device_Type
	     , Country
		 , Headquarter_ID
		 , CASE WHEN Headquarter_Name LIKE '%HQ' THEN Customer ELSE Headquarter_Name END AS Headquarter_Name
		 , CAST(COUNT(Store_Name) AS int) AS Owned_Store
		 , Currency
		 , CAST(SUM(Monthly_Fee) AS decimal(10, 2)) AS Total_Amount_Per_Month
		 , CAST(SUM(Monthly_Fee) / COUNT(Store_Name) AS decimal(10, 2)) AS Average
	FROM dbo.DMC_Monthly_Subscription(@ReportMonth)
	GROUP BY Device_Type, Country, Headquarter_ID, Customer, Headquarter_Name, Currency
)
GO
/****** Object:  Table [dbo].[TempTable_DMC_Monthly_Revenue_By_Country_Summary]    Script Date: 13/6/2025 8:49:42 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Country_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_DMC_Subscription_Revenue_By_Country_Overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_DMC_Subscription_Revenue_By_Country_Overview]
AS
SELECT [Year]
     , [COL]
	 , ISNULL([Jan], 0) AS [Jan]
	 , ISNULL([Feb], 0) AS [Feb]
	 , ISNULL([Mar], 0) AS [Mar]
	 , ISNULL([Apr], 0) AS [Apr]
	 , ISNULL([May], 0) AS [May]
	 , ISNULL([Jun], 0) AS [Jun]
	 , ISNULL([Jul], 0) AS [Jul]
	 , ISNULL([Aug], 0) AS [Aug]
	 , ISNULL([Sep], 0) AS [Sep]
	 , ISNULL([Oct], 0) AS [Oct]
	 , ISNULL([Nov], 0) AS [Nov]
	 , ISNULL([Dec], 0) AS [Dec]
	 , CASE WHEN [COL] = 'Amount' THEN (ISNULL([Jan], 0) + ISNULL([Feb], 0) + ISNULL([Mar], 0) + ISNULL([Apr], 0) + ISNULL([May], 0) + ISNULL([Jun], 0) + ISNULL([Jul], 0) + ISNULL([Aug], 0) + ISNULL([Sep], 0) + ISNULL([Oct], 0) + ISNULL([Nov], 0) + ISNULL([Dec], 0)) ELSE 0 END AS Total
FROM (
		SELECT [Year], [Month], COL, VAL FROM Temptable_DMC_Monthly_Revenue_By_Country_Summary
        CROSS APPLY (VALUES('Amount', Total_Amount), ('No of store', CAST(No_Of_Store AS int)), ('Average', Average)) CS (COL, VAL)) T
        PIVOT (MAX([VAL]) FOR [Month] IN ([Jan], [Feb], [Mar], [Apr], [May], [Jun], [Jul], [Aug], [Sep], [Oct], [Nov], [Dec])) PVT
WHERE YEAR(GETDATE()) - [Year] <= 4
GO
/****** Object:  View [dbo].[R_LMS_Module_Licence_Order]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_LMS_Module_Licence_Order]
AS
	SELECT PO_No AS [PO No]
		 , PO_Date AS [PO Date]
		 , Invoice_No AS [Invoice No]
		 , Invoice_Date AS [Invoice Date]
		 , Created_Date AS [Created Date]
		 , Chargeable
		 , Remarks
		 , Customer_ID AS [Customer ID]
		 , (SELECT Name FROM Master_Customer WHERE Customer_ID = LMS_Module_Licence_Order.Customer_ID) AS [Customer]
		 , Master_Sales_Representative.Name AS [Requested By]
		 , UID
	FROM LMS_Module_Licence_Order
	INNER JOIN Master_Sales_Representative ON Master_Sales_Representative.Sales_Representative_ID = LMS_Module_Licence_Order.Sales_Representative_ID
GO
/****** Object:  View [dbo].[D_LMS_Licence_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_LMS_Licence_Summary]
AS
SELECT [Application Type]
     , SUM(Android) AS Android
	 , SUM(iOS) AS iOS
	 , SUM(Web) AS Web
	 , SUM(SM) AS SM
FROM (
		SELECT CASE WHEN L.[Application Type Long Name] = L.[Application Type Short Name] AND L.[Application Type Short Name] = 'PC Scale' 
		            THEN LK.Value_2
					ELSE REPLACE(L.[Application Type Long Name], 'PC Scale', LK.Value_2) 
					END [Application Type]
			 , CASE WHEN L.[OS Type] = 'Android' THEN 1 ELSE 0 END AS Android
			 , CASE WHEN L.[OS Type] = 'iOS' THEN 1 ELSE 0 END AS iOS
			 , CASE WHEN L.[OS Type] = 'Web' THEN 1 ELSE 0 END AS Web
			 , CASE WHEN L.[OS Type] = 'SM' THEN 1 ELSE 0 END AS SM
        FROM DB_Lookup AS LK 
		LEFT OUTER JOIN (		
							SELECT [Licence Code], [Application Type] AS [Application Type Long Name], L.Application_Type AS [Application Type Short Name], [OS Type]
							FROM (
									SELECT [Licence Code], [Application Type], [OS Type] FROM R_LMS_Licence
									UNION
									SELECT [Licence Code], ISNULL([Application Type] + ' (' + Activated_Module_Type + ') ', [Application Type]) AS [Application Type], [OS Type] 
									FROM R_LMS_Module_Licence 
									LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.[Licence_Code] = REPLACE(R_LMS_Module_Licence.[Licence Code], '-', '') 
							) TBL 
							INNER JOIN LMS_Licence L ON L.Licence_Code = TBL.[Licence Code]
		) AS L ON L.[Application Type Short Name] = LK.Value_3
        WHERE (LK.Lookup_Name = 'Bill Items') AND LK.Value_4 IN ('App Licence', 'Module Licence Key')
) AS TBL
WHERE [Application Type] IS NOT NULL
GROUP BY [Application Type], [Android], [iOS], [Web], [SM]
GO
/****** Object:  View [dbo].[R_FTP_Server_Distributor_Account]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_FTP_Server_Distributor_Account]
AS
	SELECT ID
		 , User_Group AS [Group]
		 , Contact_Person AS [Contact Person]
		 , Email
		 , User_ID AS [User ID]
		 , User_Password AS [Password]
		 , CASE WHEN Is_Active = 1 THEN 'Active' ELSE 'Inactive' END AS [Status]
		 , Code
		 , Last_Update AS [Last Updated]
	FROM FTP_Server_Distributor_Account 
GO
/****** Object:  View [dbo].[D_LMS_Licence_Order_Outstanding_Invoice]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[D_LMS_Licence_Order_Outstanding_Invoice]
AS
SELECT [Customer ID], [Invoice Bill To], [Licensee]
	 , [PO No], [PO Date], [Invoice No], [Invoice Date]
     , (SELECT CAST(COUNT(*) AS nvarchar) FROM R_LMS_Licence WHERE [Invoice No] = '' AND [Is Cancelled] = 0 AND [PO No] = L.[PO No] AND Status = 'Activated' AND [Requested By] = L.[Requested By]) + ' / ' + CAST(COUNT([Licence Code]) AS nvarchar) AS [No of Licence]
	 , [Requested By]
	 , '' AS [Payment Status]
	 , DATEDIFF(d, (SELECT Top 1 Min([Created Date]) FROM R_LMS_Licence WHERE [PO No] = L.[PO No]), GETDATE()) AS [Days since created]
FROM R_LMS_Licence L
WHERE [Invoice No] = '' AND [Is Cancelled] = 0
GROUP BY [Customer ID], [Invoice Bill To], [Licensee], [PO No], [PO Date], [Invoice No], [Invoice Date], [Requested By]

GO
/****** Object:  View [dbo].[_Termed_Licence_Notifications_Email_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Termed_Licence_Notifications_Email_List]
AS
		WITH CTE_Sales_Rep AS (
			SELECT Sales_Representative_ID, Short_Name, Email, Supported_By, Supervised_By
			FROM Master_Sales_Representative
		),
		CTE_Support_Rep AS (
			SELECT Sales_Representative_ID, Short_Name AS Supported_Short_Name, Email AS Supported_Email
			FROM Master_Sales_Representative
		),
		CTE_Supervisor_Rep AS (
			SELECT Sales_Representative_ID, Email AS Supervisor_Email
			FROM Master_Sales_Representative
		)

		SELECT A.[Requestor ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + '; ' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM D_Licence_With_Term A
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = A.[Requestor ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		WHERE [Expired Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) - 12, 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) 
		  AND [Application Type] NOT IN ('PC Scale (AI)') AND Chargeable NOT IN ('No')  		

GO
/****** Object:  View [dbo].[R_Customer_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Customer_List]
AS
SELECT C.Customer_ID AS [Customer ID]
     , C.Name AS [Name]
	 , C.Distributor_Code [Code]
	 , C.Address AS [Address]
	 , C.Type AS [Type]
	 , C.Country AS [Country]
	 , CASE WHEN C.By_Distributor != '' THEN D.Name ELSE '' END AS [By Distributor] 
	 , G.Name AS [Group Name]
	 , (SELECT TOP 1 Headquarter_ID FROM DMC_Headquarter WHERE Customer_ID = C.Customer_ID ORDER BY Headquarter_ID DESC) AS [Headquarter ID]
FROM Master_Customer C
INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID 
LEFT JOIN Master_Customer D ON D.Customer_ID = C.By_Distributor
GO
/****** Object:  Table [dbo].[Suspended_Store]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suspended_Store](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Store_ID] [nvarchar](20) NULL,
	[Store_Name] [nvarchar](100) NULL,
	[Suspended_Date] [date] NULL,
	[Reason] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Suspended_Stores]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_Suspended_Stores]
AS
	SELECT SUBSTRING(S.Store_ID, 2, 6) AS [Headquarter ID]
		 , H.Name AS [Headquarter Name]
		 , ISNULL(S.Synced_dmcstore_userstoreid, SUBSTRING(S.Store_ID, 8, 4)) AS [Store No]
		 , SP.Store_Name AS [Store Name]
		 , (SELECT Name FROM Master_Account_Type WHERE Code = S.Account_Type) AS [Account Type]
		 , S.Created_Date AS [Created Date]
		 , CASE WHEN S.Inactive_Date IS NULL 
				THEN (SELECT MAX(End_Date) FROM DMC_Subscription WHERE Store_ID = SP.Store_ID)
				ELSE S.Inactive_Date END AS [Expiry Date]
		 , SP.Suspended_Date AS [Suspended Date]
		 , CASE WHEN DATEDIFF(d, SP.Suspended_Date, GETDATE()) >= 90 THEN 'Closed' ELSE 'Suspended' END AS [Status] 
		 , SR.Sales_Representative_ID AS [Sales Representative ID]
		 , SR.Name AS [Requestor]
		 , SP.Reason AS [Reason of Suspension]
	FROM Suspended_Store SP
	INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = SUBSTRING(SP.Store_ID, 2, 6)
	INNER JOIN DMC_Store S ON SUBSTRING(S.Store_ID, 2, 6) = SUBSTRING(SP.Store_ID, 2, 6) AND SUBSTRING(S.Store_ID, 8, 4) = SUBSTRING(SP.Store_ID, 8, 4)
	INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = (SELECT Sales_Representative_ID FROM DMC_Headquarter_Sales_Representative WHERE Headquarter_ID = SUBSTRING(S.Store_ID, 2, 6))
	WHERE S.Is_Active = 0
GO
/****** Object:  View [dbo].[D_DMC_Billed_Account_Expired_In_2_Months]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_DMC_Billed_Account_Expired_In_2_Months]
AS
SELECT [Subscription ID], ISNULL(dbo.Get_Distributor_Name(dbo.Get_Special_Arranged_Bill_Entity(TBL.[Subscription ID])), [Bill Entity]) AS [Bill Entity]
     , [Customer Name], [Group], [HQ Code], [HQ Name], [Store Code], [Store Name], [Created Date], [Start Date], [End Date], [Duration], [Currency], [Fee], [Status], [Account Type], [Sales Representative ID], [Sales Representative]
FROM (
		SELECT (SELECT TOP 1 Subscription_ID FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Subscription ID]
				, CASE WHEN C.By_Distributor = '' THEN C.Name 
					ELSE (Select Name From Master_Customer Where Customer_ID = C.By_Distributor) END AS [Bill Entity]
				, C.Name AS [Customer Name] 
				, G.Name AS [Group]
				, H.Headquarter_ID AS [HQ Code]
				, H.Name AS [HQ Name]
				, CASE WHEN S.Synced_dmcstore_userstoreid IS NOT NULL THEN CAST(Synced_dmcstore_userstoreid AS int) ELSE CAST(SUBSTRING(S.Store_ID, 8, 4) As int) END AS [Store Code]
				, S.Name AS [Store Name]
				, S.Created_Date AS [Created Date]
				, (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Start Date]
				, (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [End Date]
				, (SELECT TOP 1 Duration FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Duration]
				, (SELECT TOP 1 Currency FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Currency]
				, (SELECT TOP 1 Fee FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Fee]
				, CASE WHEN S.Is_Active = 1 THEN 'Active' ELSE 'In-Active' END AS [Status]
				, T.Name AS [Account Type]
				, (SELECT TOP 1 MR.Sales_Representative_ID 
				FROM DMC_Headquarter_Sales_Representative R
				INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
				WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
				ORDER BY R.Effective_Date DESC) AS [Sales Representative ID]
				, (SELECT TOP 1 MR.Name 
				FROM DMC_Headquarter_Sales_Representative R
				INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
				WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
				ORDER BY R.Effective_Date DESC) AS [Sales Representative]
		FROM DMC_Store S
		INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
		INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
		INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
		INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
		WHERE S.Account_Type IN ('03') AND S.Is_Active = 1
) TBL
WHERE TBL.[End Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0))  -- Change to 3 months
--WHERE TBL.[End Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 2, 0)) 

GO
/****** Object:  View [dbo].[D_DMC_Trial_Account_Expired_In_2_Months]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[D_DMC_Trial_Account_Expired_In_2_Months]
AS
SELECT * FROM (
				SELECT C.Name AS [Customer] 
					 , G.Name AS [Group]
					 , H.Headquarter_ID AS [HQ Code]
					 , H.Name AS [HQ Name]
					 --, CAST(SUBSTRING(S.Store_ID, 8, 4) As int) AS [Store Code]
					 , CASE WHEN S.Synced_dmcstore_userstoreid IS NOT NULL THEN CAST(Synced_dmcstore_userstoreid AS int) ELSE CAST(SUBSTRING(S.Store_ID, 8, 4) As int) END AS [Store Code]
					 , S.Name AS [Store Name]
					 , S.Created_Date AS [Created Date]
					 , S.Inactive_Date AS [End Date]
					 , CASE WHEN S.Is_Active = 1 THEN 'Active' ELSE 'In-Active' END AS [Status]
					 , T.Name AS [Account Type]
					 , (SELECT TOP 1 MR.Sales_Representative_ID 
					    FROM DMC_Headquarter_Sales_Representative R
						INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
						WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= S.Inactive_Date
						ORDER BY R.Effective_Date DESC) AS [Sales Representative ID]
					 , (SELECT TOP 1 MR.Name 
					    FROM DMC_Headquarter_Sales_Representative R
						INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
						WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= S.Inactive_Date
						ORDER BY R.Effective_Date DESC) AS [Sales Representative]
				FROM DMC_Store S
				INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
				INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
				INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
				INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
				WHERE S.Account_Type IN ('01') AND S.Is_Active = 1
) TBL
WHERE TBL.[End Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0))  -- Change to 3 months
--WHERE TBL.[End Date] BETWEEN DATEADD(mm, DATEDIFF(mm, 0, GETDATE()), 0) AND DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 2, 0)) 

GO
/****** Object:  View [dbo].[_DMC_Reminder_Notifications_Email_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[_DMC_Reminder_Notifications_Email_List]
AS
		WITH CTE_Sales_Rep AS (
			SELECT Sales_Representative_ID, Short_Name, Email, Supported_By, Supervised_By
			FROM Master_Sales_Representative
		),
		CTE_Support_Rep AS (
			SELECT Sales_Representative_ID, Short_Name AS Supported_Short_Name, Email AS Supported_Email
			FROM Master_Sales_Representative
		),
		CTE_Supervisor_Rep AS (
			SELECT Sales_Representative_ID, Email AS Supervisor_Email
			FROM Master_Sales_Representative
		)
		SELECT A.[Sales Representative ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + ',' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM D_DMC_Billed_Account_Expired_In_2_Months A
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = A.[Sales Representative ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		UNION
		SELECT B.[Sales Representative ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + ',' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM D_DMC_Trial_Account_Expired_In_2_Months B
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = B.[Sales Representative ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		UNION
		SELECT C.[Sales Representative ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + ',' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM R_Suspended_Stores C
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = C.[Sales Representative ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		WHERE [Suspended Date] = DATEADD(DAY, 1, EOMONTH(GETDATE(), -1));


	--SELECT A.[Sales Representative ID] AS [Recipient ID]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID])) > 0
	--			THEN B.[Short_Name] + ', ' + (SELECT Short_Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID])) 
	--			ELSE B.[Short_Name] 
	--			END AS [Recipient Name]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID])) > 0
	--			THEN B.[Email] + '; ' + (SELECT Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID]))
	--			ELSE B.[Email] 
	--			END AS [Recipient Email]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID])) > 0
	--			THEN (SELECT TOP 1 Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = A.[Sales Representative ID])) 
	--			ELSE ''
	--			END AS [Cc_Email]
	--	 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
	--FROM D_DMC_Billed_Account_Expired_In_2_Months A
	--INNER JOIN Master_Sales_Representative B ON B.Sales_Representative_ID = A.[Sales Representative ID]
	----GROUP BY A.[Sales Representative ID], B.[Email], B.[Name], B.[Short_Name]
	--UNION
	--SELECT C.[Sales Representative ID] AS [Recipient ID]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID])) > 0
	--			THEN D.[Short_Name] + ', ' + (SELECT Short_Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID])) 
	--			ELSE D.[Short_Name] 
	--			END AS [Recipient Name]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID])) > 0
	--			THEN D.[Email] + '; ' + (SELECT Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID]))
	--			ELSE D.[Email] 
	--			END AS [Recipient Email]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID])) > 0
	--			THEN (SELECT TOP 1 Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = C.[Sales Representative ID])) 
	--			ELSE ''
	--			END AS [Cc_Email]
	--	 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
	--FROM D_DMC_Trial_Account_Expired_In_2_Months C
	--INNER JOIN Master_Sales_Representative D ON D.Sales_Representative_ID = C.[Sales Representative ID]
	--UNION
	--SELECT E.[Sales Representative ID]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID])) > 0
	--			THEN F.[Short_Name] + ', ' + (SELECT Short_Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID])) 
	--			ELSE F.[Short_Name] 
	--			END AS [Recipient Name]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID])) > 0
	--			THEN F.[Email] + '; ' + (SELECT Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supported_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID]))
	--			ELSE F.[Email] 
	--			END AS [Recipient Email]
	--	 , CASE WHEN LEN((SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID])) > 0
	--			THEN (SELECT TOP 1 Email FROM Master_Sales_Representative WHERE Sales_Representative_ID = (SELECT TOP 1 Supervised_By FROM Master_Sales_Representative WHERE Sales_Representative_ID = E.[Sales Representative ID])) 
	--			ELSE ''
	--			END AS [Cc_Email]
	--	 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
	--FROM R_Suspended_Stores E
	--INNER JOIN Master_Sales_Representative F ON F.Sales_Representative_ID = E.[Sales Representative ID]
	--WHERE [Suspended Date] = DATEADD(DAY, 1, EOMONTH(GETDATE(), -1))


	-- Testing Data
	--SELECT 'S0003' AS [Recipient ID]
	--     , 'Jia Li' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
	--UNION
	--SELECT 'S0004' AS [Recipient ID]
	--     , 'Jonathan, Sim Yee' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0005' AS [Recipient ID]
	--     , 'Rodney' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0010' AS [Recipient ID]
	--     , 'Mr Tey' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0018' AS [Recipient ID]
	--     , 'Vincent' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]

GO
/****** Object:  View [dbo].[R_Maintenance_Banner]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_Banner]
AS
	SELECT Banner_ID AS [Banner ID]
	     , Banner_Name AS [Banner Name]
		 , Created_Date AS [Created Date]
		 , Last_Updated AS [Last Updated]
		 , Customer_ID AS [Customer ID]
		 , (SELECT TOP 1 Services_Group FROM Maintenance_Customer WHERE Customer_ID = Maintenance_Banner.Customer_ID) AS [Services Group]
		 , CASE WHEN (SELECT COUNT(Banner_ID) FROM Maintenance_Store WHERE Banner_ID = Maintenance_Banner.Banner_ID) > 0 THEN 1 ELSE 0 END AS [In Used]
		 , ISNULL(Frequency, '') AS [Frequency]
	FROM Maintenance_Banner
GO
/****** Object:  View [dbo].[_Database_Sessions]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Database_Sessions]
AS
	SELECT COUNT(*) AS sessions, S.host_name, S.host_process_id, S.program_name, db_name(S.database_id) AS database_name
	FROM sys.dm_exec_sessions S
	WHERE is_user_process = 1
	GROUP BY host_name, host_process_id, program_name, database_id
GO
/****** Object:  View [dbo].[_Database_Sessions_Details]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Database_Sessions_Details]
AS
	SELECT DATEDIFF(MINUTE, s.last_request_end_time, GETDATE()) AS minutes_asleep
		 , s.session_id
		 , db_name(s.database_id) AS database_name
		 , s.host_name
		 , s.host_process_id
		 , t.text as last_sql
		 , s.program_name
	FROM sys.dm_exec_connections c
	JOIN sys.dm_exec_sessions s ON c.session_id = s.session_id
	CROSS APPLY sys.dm_exec_sql_text(c.most_recent_sql_handle) t
	WHERE s.is_user_process = 1
	  AND s.status = 'sleeping'
      AND s.host_process_id = (SELECT host_process_id FROM _Database_Sessions WHERE program_name = '.Net SqlClient Data Provider')
    --AND db_name(s.database_id) = @database_name
    --AND s.host_name = @host_name
	--AND DATEDIFF(SECOND, s.last_request_end_time, GETDATE()) > 60
	--ORDER BY s.last_request_end_time;
	--ORDER BY session_id
GO
/****** Object:  Table [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_DMC_Subscription_Revenue_By_Account_Type_Overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_DMC_Subscription_Revenue_By_Account_Type_Overview]
AS
SELECT [Year]
     , [COL]
	 , ISNULL([Jan], 0) AS [Jan]
	 , ISNULL([Feb], 0) AS [Feb]
	 , ISNULL([Mar], 0) AS [Mar]
	 , ISNULL([Apr], 0) AS [Apr]
	 , ISNULL([May], 0) AS [May]
	 , ISNULL([Jun], 0) AS [Jun]
	 , ISNULL([Jul], 0) AS [Jul]
	 , ISNULL([Aug], 0) AS [Aug]
	 , ISNULL([Sep], 0) AS [Sep]
	 , ISNULL([Oct], 0) AS [Oct]
	 , ISNULL([Nov], 0) AS [Nov]
	 , ISNULL([Dec], 0) AS [Dec]
	 , CASE WHEN [COL] = 'Amount' THEN (ISNULL([Jan], 0) + ISNULL([Feb], 0) + ISNULL([Mar], 0) + ISNULL([Apr], 0) + ISNULL([May], 0) + ISNULL([Jun], 0) + ISNULL([Jul], 0) + ISNULL([Aug], 0) + ISNULL([Sep], 0) + ISNULL([Oct], 0) + ISNULL([Nov], 0) + ISNULL([Dec], 0)) ELSE 0 END AS Total
FROM (
		SELECT [Year], [Month], COL, VAL FROM TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary
        CROSS APPLY (VALUES('Amount', Total_Amount), ('No of store', CAST(No_Of_Store AS int)), ('Average', Average)) CS (COL, VAL)) T
        PIVOT (MAX([VAL]) FOR [Month] IN ([Jan], [Feb], [Mar], [Apr], [May], [Jun], [Jul], [Aug], [Sep], [Oct], [Nov], [Dec])) PVT
WHERE YEAR(GETDATE()) - [Year] <= 4
GO
/****** Object:  Table [dbo].[CZL_Device_Status_Log]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Device_Status_Log](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Created_Date] [date] NULL,
	[Log_Description] [nvarchar](max) NULL,
	[Unique_ID] [nvarchar](20) NULL,
	[Log_Type] [nvarchar](3) NULL,
	[Last_Update] [date] NULL,
	[By_Who] [nvarchar](50) NULL,
	[UID] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_CZL_Device_Status_Log]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_CZL_Device_Status_Log]
AS
SELECT ID, Created_Date, Log_Description, Unique_ID
     , CASE WHEN DATEDIFF(d, Created_Date, GETDATE()) > 90 THEN 'SYS' ELSE Log_Type END AS Log_Type
	 , Last_Update, By_Who, UID
     , 'Added by ' + By_Who + ' (' + CAST(Created_Date AS nvarchar) + ')' AS Added_By
     , CASE WHEN DATEDIFF(d, Created_Date, Last_Update) > 0 THEN 1 ELSE 0 END AS IsEdited
	 , CASE WHEN Log_Type != 'SYS' 
	        THEN CASE WHEN DATEDIFF(d, Created_Date, Last_Update) > 0 THEN 'Edited by ' + By_Who + ' (' + CAST(Last_Update AS nvarchar) + ')' ELSE NULL END 
			ELSE NULL 
			END AS Edited_By
FROM CZL_Device_Status_Log
GO
/****** Object:  View [dbo].[R_Headquarter_Sales_Representative]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Headquarter_Sales_Representative]
AS
SELECT H.Customer_ID AS [Customer ID]
     , A.Headquarter_ID AS [Headquarter ID]
	 , H.Name AS [Headquarter Name]
	 , S.Name AS [Requested By]
	 , H.Created_Date AS [Created Date]
	 , B.Effective_Date AS [Effective Date]
     , CASE WHEN H.Is_Active = 0 THEN 'Inactive' ELSE 'Active' END AS [Status]
	 , H.Inactive_Date AS [Inactive Date]
	 , CASE WHEN (SELECT COUNT(Headquarter_ID) FROM DMC_Store WHERE Headquarter_ID = A.Headquarter_ID) > 0 THEN 1 ELSE 0 END AS [In Used]
FROM DMC_Headquarter_Sales_Representative A
INNER JOIN (
			SELECT Headquarter_ID, MAX(Effective_Date) AS Effective_Date 
			FROM DMC_Headquarter_Sales_Representative
			GROUP BY Headquarter_ID
			) B ON B.Headquarter_ID = A.Headquarter_ID AND B.Effective_Date = A.Effective_Date
INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = A.Headquarter_ID
INNER JOIN Master_Sales_Representative S ON S.Sales_Representative_ID = A.Sales_Representative_ID
GO
/****** Object:  Table [dbo].[DMC_Maintenance_History]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Maintenance_History](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Maintenance_Date] [date] NULL,
	[Work_Type] [nvarchar](50) NULL,
	[Description] [nvarchar](100) NULL,
	[Backup_FileName] [nvarchar](100) NULL,
	[Backup_FileSize] [nvarchar](50) NULL,
	[opt_database_size] [nvarchar](50) NULL,
	[Status] [nvarchar](50) NULL,
	[Downtime_From] [nvarchar](50) NULL,
	[Downtime_To] [nvarchar](50) NULL,
	[Duration] [nvarchar](50) NULL,
	[Remarks] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Maintenance_History_Report]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Maintenance_History_Report](@ReportYear date) RETURNS TABLE 
AS
RETURN 
(
	SELECT Maintenance_Date AS [Maintenance Date]
 	     , Work_Type AS [Work Type]
		 , Description
		 , Downtime_From AS [Down Time From]
		 , Downtime_To AS [Down Time To]
		 , CASE WHEN CAST(Downtime_To AS TIME) < CAST(Downtime_From AS TIME) 
		        THEN -- When Downtime_To is earlier than Downtime_From, it indicates crossing to the next day
			         DATEDIFF(MINUTE, CAST('1900-01-01 ' + Downtime_From AS DATETIME), CAST('1900-01-02 ' + Downtime_To AS DATETIME))
                ELSE
                     DATEDIFF(MINUTE, CAST('1900-01-01 ' + Downtime_From AS DATETIME), CAST('1900-01-01 ' + Downtime_To AS DATETIME))
		   END AS Duration 
	FROM DMC_Maintenance_History
	WHERE YEAR(Maintenance_Date) = YEAR(@ReportYear)
	  AND LEN(Downtime_From) > 0 AND LEN(Downtime_To) > 0
	  AND Status IN ('Completed')
)
GO
/****** Object:  View [dbo].[R_DMC_Store_Licence]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_DMC_Store_Licence]
AS
	SELECT H.Headquarter_ID AS [Headquarter ID]
	     , H.Name AS [Headquarter Name]
		 , ISNULL(S.Synced_dmcstore_userstoreid, SUBSTRING(Store_ID, 8, 4)) AS [Store No]
		 , S.Name AS [Store Name]
		 , S.Banner AS [Banner]
		 , S.Zone AS [Zone]
		 , S.Account_Type AS [Type Code]
		 , (SELECT Name FROM Master_Account_Type WHERE Code = S.Account_Type) AS [Account Type]
		 , S.Created_Date AS [Created Date]
		 , S.Effective_Date AS [Effective Date]
		 , ISNULL(S.Effective_Date, dbo.Get_Subscription_Start_Date(S.Store_ID)) AS [Start Date]
		 , ISNULL(S.Inactive_Date, dbo.Get_Subscription_End_Date(S.Store_ID)) AS [End Date]
		 , S.Public_IP AS [Public IP]
		 , S.FTP_Host AS [FTP Host]
		 , S.FTP_User AS [FTP User]
		 , S.FTP_Password AS [FTP Password]
		 , CASE WHEN S.Is_Active = 1 THEN 'Active' 
				ELSE CASE WHEN DATEDIFF(d, ISNULL(S.Inactive_Date, dbo.Get_Subscription_End_Date(S.Store_ID)), GETDATE()) > 90 THEN 'Closed' ELSE 'Suspended' END
				END [Status]
		 , S.Store_ID AS [Store ID]
		 , S.Synced_dmcstore_id
		 , H.Customer_ID AS [Customer ID]
		 , C.Name AS [Customer]
		 , S.ID
		 , CASE WHEN (SELECT COUNT(Store_ID) FROM DMC_Subscription WHERE Store_ID = S.Store_ID) > 0 THEN 1 
		        ELSE CASE WHEN (SELECT Name FROM Master_Account_Type WHERE Code = S.Account_Type) != 'Billed' THEN 1 ELSE 0 END 
				END AS [In Used]
	FROM DMC_Store S
	LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
    INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
GO
/****** Object:  Table [dbo].[Maintenance_ESL_Tags]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_ESL_Tags](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Unique_ID] [nvarchar](20) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Installation_Date] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_ESL_Tags]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_ESL_Tags]
AS
SELECT A.Unique_ID AS [Unique ID]
     , A.Customer_ID AS [Customer ID]
	 , C.Name AS [Customer Name]
	 , A.Store_ID AS [Store ID]
	 , B.Store_Name AS [Store Name]
	 , A.Installation_Date AS [Installation Date]
	 , C.Services_Group AS [Services Group]
	 , CASE WHEN DATEDIFF(Month, A.Installation_Date, GETDATE()) > 6 THEN 0 ELSE 1 END [Editable] 
FROM Maintenance_ESL_Tags A
LEFT JOIN Maintenance_Store B ON B.Store_ID = A.Store_ID AND B.Customer_ID = A.Customer_ID
LEFT JOIN Maintenance_Customer C ON C.Customer_ID = A.Customer_ID

GO
/****** Object:  View [dbo].[_LMS_Licence_Details]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[_LMS_Licence_Details]
AS
SELECT L.Customer_ID AS [Customer ID]
     , C.Name AS [Licensee]
	 , ISNULL((SELECT Name FROM Master_Customer WHERE Customer_ID = C.By_Distributor), C.Name) AS [Invoice Bill To]
     , L.PO_No AS [PO No]
	 , L.PO_Date AS [PO Date]
	 , L.Invoice_No AS [Invoice No]
	 , L.Invoice_Date AS [Invoice Date]
	 , ISNULL((SELECT Value_2 FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_4 IN ('App Licence', 'DMC Server Licence Key') AND Value_3 = L.Application_Type), ISNULL(L.Application_Type + ' (' + Activated_Module_Type + ') ', L.Application_Type)) AS [Application Type]
	 , L.OS_Type AS [OS Type]
	 , L.Licence_Code AS [Licence Code]
	 , L.Synced_dmcmobiletoken_term AS [Licence Term]
	 , L.Synced_dmcmobiletoken_maxhq AS [Max HQ]
	 , L.Synced_dmcmobiletoken_maxstore AS [Max Store]
	 , L.Serial_No AS [Serial No]
	 , L.Synced_dmcmobiletoken_unique_id AS [MAC Address]
	 , L.AI_Device_ID AS [AI Device ID]
	 , L.AI_Device_Serial_No AS [AI Device Serial No]
	 , L.Created_Date AS [Created Date]
	 , L.Synced_dmcmobiletoken_activateddate AS [Activated Date]
	 , CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
	        THEN 'No Expiry' 
			ELSE CONVERT(nvarchar, DATEADD(Day, -1, L.Synced_dmcmobiletoken_expireddate), 23) 
			END AS [Expired Date]
	 , CASE WHEN L.Synced_dmcmobiletoken_status != 'Blocked'
		    THEN CASE WHEN DATEDIFF(YEAR, L.Synced_dmcmobiletoken_activateddate, L.Synced_dmcmobiletoken_expireddate) > 50 
				      THEN L.Synced_dmcmobiletoken_status
					  ELSE CASE WHEN L.Synced_dmcmobiletoken_expireddate < GETDATE() AND L.Synced_dmcmobiletoken_status != 'Renew' THEN 'Expired' ELSE L.Synced_dmcmobiletoken_status END
					  END
		    ELSE L.Synced_dmcmobiletoken_status
		    END AS [Status]
	 , L.Licensee_Email AS [Email]
	 , SR.Name AS [Requested By]
	 , CASE WHEN L.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable]
	 , L.Remarks 
FROM LMS_Licence AS L 
INNER JOIN Master_Customer AS C ON C.Customer_ID = L.Customer_ID 
INNER JOIN Master_Sales_Representative SR ON SR.Sales_Representative_ID = L.Sales_Representative_ID
LEFT JOIN LMS_Module_Licence_Activated ON LMS_Module_Licence_Activated.Licence_Code = REPLACE(L.Licence_Code, '-', '')
GO
/****** Object:  View [dbo].[R_DMC_User_Licence]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_DMC_User_Licence]
AS
SELECT U.Headquarter_ID AS [Headquarter ID]
     , H.Name AS [Headquarter Name]
     , Username AS [Username]
     , Password AS [Password]
	 , U.Email AS [Email]
	 , U.Created_Date AS [Created Date]
	 , Effective_Date AS [Effective Date]
     , CASE WHEN U.Is_Active = 1 THEN 'Active' 
	        ELSE CASE WHEN DATEDIFF(d, U.Inactive_Date, GETDATE()) > 90 THEN 'Closed' ELSE 'Suspended' END
		    END [Status]
	 , U.Inactive_Date AS [Inactive Date]
	 , CASE Synced_dmcuser_devicetype WHEN 0 THEN 'All' WHEN 1 THEN 'POS' WHEN 2 THEN 'Retail' END AS [Device Type]
	 , H.Customer_ID AS [Customer ID]
	 , C.Name AS [Customer]
	 , U.ID
     , CASE WHEN (SELECT COUNT(Store_ID) FROM DMC_Subscription WHERE SUBSTRING(Store_ID, 2, 6) = U.Headquarter_ID) > 0 THEN 1 ELSE 0 END AS [In Used]
FROM DMC_User U
LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = U.Headquarter_ID
INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID

GO
/****** Object:  Table [dbo].[Maintenance_ESL_Tags_Type]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_ESL_Tags_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Tags_Group] [nvarchar](20) NOT NULL,
	[Tags_Type] [nvarchar](10) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_ESL_Tags_Deployment]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_ESL_Tags_Deployment](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Unique_ID] [nvarchar](20) NOT NULL,
	[Tags_Group] [nvarchar](20) NOT NULL,
	[Tags_Type] [nvarchar](10) NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_ESL_Tags_Type]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[R_Maintenance_ESL_Tags_Type]
AS
SELECT ID AS [ID]
     , Tags_Group AS [Tags Group]
     , Tags_Type AS [Tags Type]
	 , CASE WHEN (SELECT COUNT(Tags_Type) FROM Maintenance_ESL_Tags_Deployment WHERE Tags_Type = Maintenance_ESL_Tags_Type.Tags_Type) > 0 THEN 1 ELSE 0 END AS [In Used]
FROM Maintenance_ESL_Tags_Type

GO
/****** Object:  View [dbo].[R_DMC_Subscription]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_DMC_Subscription]
AS
SELECT Subscription_ID AS [Subscription ID]
	 , SUBSTRING(Store_ID, 2, 6) AS [Headquarter ID]
	 , H.Name AS [Headquarter Name]
	 , ISNULL(Ref_Invoice_No, '') AS [Invoice No]
	 , S.Invoiced_Date AS [Invoice Date]
	 , Currency
	 , ROUND(SUM(Fee), 2) AS Fee
	 , Payment_Method AS [Payment Method]
	 , Payment_Mode AS [Payment Mode]
	 , Payment_Status AS [Payment Status]
	 , CASE WHEN S.End_Date > GETDATE() - 1 
	        THEN CASE WHEN S.Start_Date > GETDATE() THEN 'New' ELSE 'In-force' END  
			ELSE 'Expired' END AS [Status]
	 , H.Customer_ID AS [Customer ID]
FROM DMC_Subscription S
LEFT JOIN DMC_Headquarter H ON H.Headquarter_ID = SUBSTRING(Store_ID, 2, 6)
--WHERE SUBSTRING(Subscription_ID, 5, 4) > YEAR(GETDATE()) - 3
GROUP BY Subscription_ID, SUBSTRING(Store_ID, 2, 6), H.Name, Ref_Invoice_No, Invoiced_Date, Currency, Payment_Method, Payment_Mode, S.Start_Date, S.End_Date, Payment_Status, H.Customer_ID

GO
/****** Object:  View [dbo].[_AccountByGroup]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_AccountByGroup]
AS
	SELECT G.Name AS [Group Name], S.Account_Type AS [Account Type], S.Is_Active AS [Status], S.End_Date AS [End Date], COUNT(S.Store_ID) AS [Quantity]
	FROM DMC_Headquarter H
	INNER JOIN (
					SELECT Store_ID
							, CASE WHEN dbo.Get_Account_Type(Account_Type) = 'Test' THEN 'Demo' ELSE dbo.Get_Account_Type(Account_Type) END Account_Type
							, CASE WHEN Inactive_Date IS NULL THEN dbo.Get_Subscription_End_Date(Store_ID) ELSE Inactive_Date END AS End_Date
							, Is_Active
							, Headquarter_ID
					FROM DMC_Store
				) S ON S.Headquarter_ID = H.Headquarter_ID
	INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
	INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
	GROUP BY G.Name, S.Account_Type, S.Is_Active, S.End_Date
GO
/****** Object:  View [dbo].[D_DMC_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_DMC_Summary]
AS
SELECT [Group Name]
     , HQ
	 , Store
	 , ISNULL([Demo Store], 0) AS [Demo]
	 , ISNULL([Billed Store], 0) AS [Billed]
	 , ISNULL([Trial Store], 0) AS [Trial]
     , ISNULL([In-Active Store], 0) AS [Suspended]
	 , ISNULL([Closed Store], 0) AS [Closed Store]
	 , ISNULL([Closed Demo], 0) AS [Closed Demo]
	 , ISNULL([Closed Billed], 0) AS [Closed Billed]
	 , ISNULL([Closed Trial], 0) AS [Closed Trial]
FROM (
			SELECT G.Name AS [Group Name], 'HQ' AS Category, COUNT(H.Headquarter_ID) AS [Quantity]
			FROM DMC_Headquarter H
			INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
			INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
			GROUP BY G.Name
			UNION ALL
			SELECT [Group Name], 'Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Demo Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Status] = 1 AND [Account Type] IN ('Test', 'Demo')
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Billed Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Status] = 1 AND [Account Type] IN ('Billed')
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Trial Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Status] = 1 AND [Account Type] IN ('Trial')
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'In-Active Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Status] = 0 AND [End Date] IS NOT NULL AND DATEDIFF(d, [End Date], GETDATE()) <= 90
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Closed Store' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Status] = 0 AND [End Date] IS NOT NULL AND DATEDIFF(d, [End Date], GETDATE()) > 90
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Closed Demo' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Account Type] IN ('Test', 'Demo') AND [Status] = 0 AND [End Date] IS NOT NULL AND DATEDIFF(d, [End Date], GETDATE()) > 90
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Closed Billed' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Account Type] IN ('Billed') AND [Status] = 0 AND [End Date] IS NOT NULL AND DATEDIFF(d, [End Date], GETDATE()) > 90
			GROUP BY [Group Name]
			UNION ALL
			SELECT [Group Name], 'Closed Trial' AS Category, SUM([Quantity]) AS [Quantity]
			FROM _AccountByGroup
			WHERE [Account Type] IN ('Trial') AND [Status] = 0 AND [End Date] IS NOT NULL AND DATEDIFF(d, [End Date], GETDATE()) > 90
			GROUP BY [Group Name]
	) SRC
	PIVOT
	(
		SUM(Quantity)
		FOR Category IN (HQ, Store, [In-Active Store], [Demo Store], [Billed Store], [Trial Store], [Closed Store], [Closed Demo], [Closed Billed], [Closed Trial])
	) PVT

GO
/****** Object:  UserDefinedFunction [dbo].[Maintenance_Monthly_Revenue]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Maintenance_Monthly_Revenue](@ReportMonth date, @Service_Group nvarchar(1)) RETURNS TABLE 
AS
RETURN 
(
	SELECT *
		 , ROUND(([Daily Rate] * ([Valid Days] - IIF(MONTH([Report End Date]) = 2 AND YEAR([Report End Date]) % 4 = 0 AND (YEAR([Report End Date]) % 100 != 0 OR YEAR([Report End Date]) % 400 = 0), 1, 0))), 4) AS [Amount On Month]   -- Deduct one day for Feb is Leap Year
		 , CASE WHEN [Valid Days] < DAY([Report End Date]) THEN '(Prorated)' ELSE '' END [Is Prorated]
	FROM (  SELECT R.[Unique ID]
	             , R.[Reference No]
				 , R.[Customer ID]
				 , R.[Customer Name]
				 , R.[Services Group]
				 , R.[Store ID]
				 , R.[Store Name]
				 , R.[Start Date]
				 , R.[End Date]
				 , CASE WHEN Currency != 'SGD' THEN 'SGD' ELSE Currency END AS [Currency]
				 , R.[Amount] AS [Contract Value]
				 , DATEDIFF(DAY, R.[Start Date], R.[End Date]) AS [Period In Days]
				 --, ROUND(R.[Amount] / DATEDIFF(DAY, R.[Start Date], R.[End Date]), 4) AS [Daily Rate]
				 , ROUND(R.[Amount] / NULLIF(DATEDIFF(DAY, R.[Start Date], R.[End Date]) - IIF(YEAR(R.[End Date]) % 4 = 0 AND (YEAR(R.[End Date]) % 100 != 0 OR YEAR(R.[End Date]) % 400 = 0), 1, 0), 0), 4) AS [Daily Rate]
				 , DATEADD(Month, DATEDIFF(month, 0, @ReportMonth), 0) AS [Report Start Date]
				 , DATEADD(Day, -1, DATEADD(month, DATEDIFF(month, 0, @ReportMonth) + 1, 0)) AS [Report End Date]
				 , dbo.Get_NumberOfMonth(R.[Start Date], R.[End Date]) AS [No of Months]
				 , CASE WHEN DATEDIFF(Month, @ReportMonth, R.[End Date]) <= dbo.Get_NumberOfMonth(R.[Start Date], R.[End Date])
						THEN CASE WHEN DATEDIFF(Month, @ReportMonth, R.[End Date]) > 0 THEN DATEDIFF(Month, @ReportMonth, R.[End Date]) ELSE 0 END 
						ELSE dbo.Get_NumberOfMonth(R.[Start Date], R.[End Date]) END AS [Remaining Cycle]
				 , CASE WHEN YEAR(R.[Start Date]) = YEAR(@ReportMonth) AND MONTH(R.[Start Date]) =  MONTH(@ReportMonth)
						THEN DATEDIFF(DAY, DATEADD(DAY, -1, R.[Start Date]), EOMONTH(@ReportMonth))
						ELSE CASE WHEN YEAR(R.[End Date]) = YEAR(@ReportMonth) AND MONTH(R.[End Date]) =  MONTH(@ReportMonth)
								  THEN DAY(EOMONTH(@ReportMonth)) - DATEDIFF(DAY, R.[End Date], EOMONTH(@ReportMonth)) 
								  ELSE DAY(EOMONTH(@ReportMonth)) 
								  END  
						END AS [Valid Days]
			 FROM R_Maintenance_Contract R
			 WHERE [Services Group] = @Service_Group
	    ) TBL
	WHERE NOT ([Start Date] >= [Report End Date] OR [End Date] <= [Report Start Date])  
)
GO
/****** Object:  Table [dbo].[LMS_AI_Gateway_Licence]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_AI_Gateway_Licence](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[Synced_dmclicensetoken_Token] [nvarchar](50) NULL,
	[Synced_dmclicensetoken_createdate] [date] NULL,
	[Synced_dmclicensetoken_activatedate] [date] NULL,
	[Synced_dmclicensetoken_expiredate] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_AI_Gateway_Licence]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_AI_Gateway_Licence]
AS
	SELECT B.Customer_ID AS [Customer ID]
	     , B.PO_No AS [PO No]
		 , B.PO_Date AS [PO Date]
		 , B.Licence_Code AS [Licence Code]
		 , A.Synced_dmclicensetoken_Token AS [Token]
		 , A.Synced_dmclicensetoken_createdate AS [Created Date]
		 , A.Synced_dmclicensetoken_activatedate AS [Activated Date]
		 , A.Synced_dmclicensetoken_expiredate AS [Expired Date]
		 , C.Client_ID AS [AI Account No]
		 , CASE WHEN A.Synced_dmclicensetoken_activatedate IS NOT NULL
				THEN CASE WHEN A.Synced_dmclicensetoken_expiredate > DATEADD(DAY, -1, GETDATE()) THEN 'Activated' ELSE 'Expired' END
				--ELSE CASE WHEN A.Synced_dmclicensetoken_expiredate > DATEADD(DAY, -1, GETDATE()) THEN 'Valid' ELSE 'Expired Without Activation' END
				ELSE 'Valid'
				END [Status]
	FROM LMS_AI_Gateway_Licence A
	INNER JOIN LMS_Licence B ON REPLACE(B.Licence_Code, '-', '') = A.Licence_Code
	LEFT JOIN CZL_Account C ON REPLACE(C.AI_Gateway_Key, '-', '') = A.Licence_Code
GO
/****** Object:  View [dbo].[L_dmclicensemoduleassign]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[L_dmclicensemoduleassign]
AS
	-- This view shows which module licence type a licence key activated
	SELECT id, hqid, storeid, dmcmobiletokenlicensecode, dmcapimoduletypename
	FROM OPENQUERY(DMCLIVE, 'Select id, hqid, storeid, dmcmobiletokenlicensecode, dmcapimoduletypename from dmclicensemoduleassign') AS derived_dmclicensemoduleassign
GO
/****** Object:  View [dbo].[L_dmclicensemoduledetail]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[L_dmclicensemoduledetail]
AS
	-- This view retrieve module licence pool from DMC
	SELECT hqid, storeid, dmcapimoduletypename, qty
	FROM OPENQUERY(DMCLIVE, 'Select hqid, storeid, dmcapimoduletypename, qty from dmclicensemoduledetail') AS derived_dmclicensemoduledetail
GO
/****** Object:  View [dbo].[L_dmchqstore]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_dmchqstore]
AS
SELECT  hqid AS [HQ Code]
      , hqname AS [HQ Name]
	  , bannername AS [Banner]
	  , db_store_id AS [DB Store ID]
	  , storeid AS [Store Code]
	  , storename AS [Store Name]
	  , lastuseddate AS [Last Used Date]
	  , updates AS [Deleted]
	  , accountstatus AS [Status]
FROM  OPENQUERY(DMCLIVE, 
                         'select H.id AS hqid
                              , H.name AS hqname
							  , S.bannerid AS bannerid
							  , B.name AS bannername
							  , S.id AS DB_Store_ID
                              , S.userstoreid AS storeid
                              , S.name AS storename
                              , S.saleslastuseddate AS lastuseddate
                              , S.updates
							  , S.Acccounttype AS accountstatus
                         from dmcstore S 
                         inner join dmchq H on H.id = S.hqid
						 inner join dmcbanner B on B.id = S.bannerid
                         order by H.id, S.userstoreid, S.name'
				) AS deriveddmchqstore
GO
/****** Object:  View [dbo].[L_dmcmodulelicensepool]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_dmcmodulelicensepool]
AS
	SELECT DISTINCT A.hqid, hs.[HQ Name]
		 , A.storeid
		 , hs.[Store Code]
		 , hs.[Store Name]
		 , A.dmcapimoduletypename AS [Module Type]
		 , ISNULL(A.balance, 0) AS Balance
		 , ISNULL(B.used, 0) AS Used 
	FROM (
			SELECT hqid, storeid, dmcapimoduletypename, SUM(qty) AS balance
			FROM L_dmclicensemoduledetail
			GROUP BY hqid, storeid, dmcapimoduletypename
		 ) A
	LEFT JOIN (
				 SELECT hqid, storeid, dmcapimoduletypename, SUM(used) as Used 
                 FROM (
		                SELECT hqid, storeid, dmcapimoduletypename, count(dmcapimoduletypename) as used
		                FROM L_dmclicensemoduleassign
		                GROUP BY id, hqid, storeid, dmcapimoduletypename
                      ) TBL
                 GROUP BY hqid, storeid, dmcapimoduletypename
			   ) B on B.hqid = A.hqid AND B.storeid = A.storeid AND B.dmcapimoduletypename = A.dmcapimoduletypename
	INNER JOIN L_dmchqstore hs ON hs.[HQ Code] = A.hqid AND hs.[DB Store ID] = A.storeid

GO
/****** Object:  View [dbo].[_HardkeyByCountry]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_HardkeyByCountry]
AS
	SELECT C.Country, PLU_Code, COUNT(HL.PLU_Code) AS Total
	FROM LMS_Hardkey_Licence AS HL 
	INNER JOIN Master_Customer AS C ON C.Customer_ID = HL.Customer_ID
	GROUP BY C.Country, PLU_Code
GO
/****** Object:  View [dbo].[R_Maintenance_ESL_Tags_Deployment]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_ESL_Tags_Deployment]
AS
SELECT Maintenance_ESL_Tags_Deployment.Unique_ID AS [Unique ID]
     , Tags_Group AS [Tags Group]
     , Tags_Type AS [Tags Type]
	 , Tags_Group + '_' + Tags_Type AS [Tags Category]
	 , Quantity AS [Quantity]
	 , Maintenance_ESL_Tags.Customer_ID AS [Customer ID]
	 , Maintenance_ESL_Tags.Store_ID AS [Store ID]
FROM Maintenance_ESL_Tags_Deployment
LEFT JOIN Maintenance_ESL_Tags ON Maintenance_ESL_Tags.Unique_ID = Maintenance_ESL_Tags_Deployment.Unique_ID
GO
/****** Object:  View [dbo].[D_Hardkey_Licence_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[D_Hardkey_Licence_Summary]
AS
SELECT Country
     , ISNULL([Demo], 0) AS [Demo]
	 , ISNULL([Tier 1], 0) AS [Tier 1]
	 , ISNULL([Tier 2], 0) AS [Tier 2]
	 , ISNULL([Tier 3], 0) AS [Tier 3]
	 , ISNULL([Tier 4], 0) AS [Tier 4]
	 , ISNULL([Tier 5], 0) AS [Tier 5]
	 , ISNULL([CMS], 0) AS [CMS]
	 , ISNULL([CMS Demo], 0) AS [CMS Demo]
	 , ISNULL([Total], 0) AS [Total]
FROM (
	SELECT Country, 'Total' AS Category, Total AS Quantity FROM _HardkeyByCountry
	UNION ALL
	SELECT Country, 'Demo' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64150'
	UNION ALL
	SELECT Country, 'Tier 1' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64151'
	UNION ALL
	SELECT Country, 'Tier 2' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64152'
	UNION ALL
	SELECT Country, 'Tier 3' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64153'
	UNION ALL
	SELECT Country, 'Tier 4' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64154'
	UNION ALL
	SELECT Country, 'Tier 5' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64155'
	UNION ALL
	SELECT Country, 'CMS' AS Category, Total AS Quantity FROM _HardkeyByCountry WHERE PLU_Code = '64391'
	UNION ALL
	SELECT Country, 'CMS Demo' AS Category, Total AS Quantity FROM  _HardkeyByCountry WHERE PLU_Code = '64392'
    ) SRC
	PIVOT
	(
		SUM(Quantity)
		FOR Category IN ([Demo], [Tier 1], [Tier 2], [Tier 3], [Tier 4], [Tier 5], [CMS], [CMS Demo], [Total])
	) PVT

GO
/****** Object:  View [dbo].[R_LMS_Licence_Order]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[R_LMS_Licence_Order]
AS
	--SELECT [Customer ID], [PO No], [PO Date], [Invoice No], STRING_AGG([Requested By], ', ') AS [Requested By], [No of Licence Key Issued]
	--SELECT [Customer ID], [PO No], [PO Date], STRING_AGG([Invoice No], ', ') AS [Invoice No], STRING_AGG([Requested By], ', ') AS [Requested By], [No of Licence Key Issued]
	--FROM (
	--		SELECT [Customer ID], [PO No], [PO Date], [Invoice No]
	--			 , CASE WHEN [Invoice No] = 'NA' THEN '' ELSE [Requested By] END AS [Requested By]
	--			 , (SELECT CAST(COUNT(*) AS nvarchar) FROM R_LMS_Licence WHERE [Customer ID] = L.[Customer ID] AND [Is Cancelled] = 0 AND [PO No] = L.[PO No] AND Status = 'Activated') + ' / ' 
	--			 + (SELECT CAST(COUNT(*) AS nvarchar) FROM R_LMS_Licence WHERE [Customer ID] = L.[Customer ID] AND [Is Cancelled] = 0 AND [PO No] = L.[PO No]) AS [No of Licence Key Issued] 
	--		FROM R_LMS_Licence L 
	--		GROUP BY [Customer ID], [PO No], [PO Date], [Invoice No], [Invoice Date], CASE WHEN [Invoice No] = 'NA' THEN '' ELSE [Requested By] END 
	--) TBL
	--GROUP BY [Customer ID], [PO No], [PO Date], [No of Licence Key Issued]

	WITH CTE AS (
		SELECT [Customer ID], [PO No], [PO Date], [Invoice No], 
			   CASE WHEN [Invoice No] = 'NA' THEN '' ELSE [Requested By] END AS [Requested By],
			   ROW_NUMBER() OVER (PARTITION BY [Customer ID], [PO No], [Invoice No] ORDER BY [Invoice No]) AS RowNum,
			   (SELECT CAST(COUNT(*) AS nvarchar) 
				FROM R_LMS_Licence 
				WHERE [Customer ID] = L.[Customer ID] AND [Is Cancelled] = 0 AND [PO No] = L.[PO No] AND Status = 'Activated') + ' / ' 
			   + (SELECT CAST(COUNT(*) AS nvarchar) 
				  FROM R_LMS_Licence 
				  WHERE [Customer ID] = L.[Customer ID] AND [Is Cancelled] = 0 AND [PO No] = L.[PO No]) AS [No of Licence Key Issued] 
		FROM R_LMS_Licence L
	)
	SELECT [Customer ID], [PO No], [PO Date], 
		   STRING_AGG([Invoice No], ', ') AS [Invoice No], 
		   STRING_AGG([Requested By], ', ') AS [Requested By], 
		   [No of Licence Key Issued]
	FROM CTE
	WHERE RowNum = 1
	GROUP BY [Customer ID], [PO No], [PO Date], [No of Licence Key Issued];

GO
/****** Object:  View [dbo].[_Server_Space_Month_Moving_Average]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Server_Space_Month_Moving_Average]
AS
	WITH CTE AS (
					SELECT A.[Start Date]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Month A
					LEFT JOIN _Server_Space_Month B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Start Date], CAST(A.[Start Date] AS nvarchar)
	) 

SELECT [Start Date]
	 , [Used]
	 , [Used Growth]
	 --, CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , [DB Size]
	 , [DB Growth]
	 --, CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [DB Growth Moving Average]
	 , CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [DB Growth Moving Average]
FROM CTE;

GO
/****** Object:  Table [dbo].[CZL_Account_Model_Upgrade_Charge]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Account_Model_Upgrade_Charge](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NOT NULL,
	[Client_ID] [nvarchar](5) NULL,
	[Upgraded_Model] [nvarchar](10) NULL,
	[Upgraded_Date] [date] NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Distributor_ID] [nvarchar](20) NULL,
	[Requested_By] [nvarchar](100) NULL,
	[Bind_Key] [nvarchar](50) NULL,
	[UID] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[I_CZL_Account_Model_Upgrade_Charge]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[I_CZL_Account_Model_Upgrade_Charge]
AS
	SELECT A.ID
	     , A.CZL_Account_Unique_ID AS [CZL Account ID]
	     , A.Client_ID AS [CZL Client ID]
		 , B.[User Group] AS [Account Name]
		 , A.Upgraded_Model AS [Upgraded Model]
		 , A.Upgraded_Date AS [Upgrade Date]
		 , A.PO_No AS [PO No]
		 , A.PO_Date AS [PO Date]
		 , A.Invoice_No AS [Invoice No]
		 , A.Invoice_Date AS [Invoice Date]
		 , CASE WHEN A.Chargeable = NULL 
		        THEN CASE WHEN A.Chargeable = 1 THEN 'Yes' ELSE 'No' END
				ELSE A.Chargeable END AS [Chargeable]
		 , A.Currency AS [Currency]
		 , A.Fee AS [Fee]
		 , A.Distributor_ID AS [Distributor ID]
		 , C.Name AS [Distributor]
		 , A.Requested_By AS [Requested By]
		 , A.UID AS [UID]
	FROM CZL_Account_Model_Upgrade_Charge A
	INNER JOIN R_CZL_Account B ON B.[CZL Account ID] = A.CZL_Account_Unique_ID AND B.[CZL Client ID] = A.Client_ID
	INNER JOIN Master_Customer C ON C.Customer_ID = A.Distributor_ID
GO
/****** Object:  View [dbo].[R_Maintenance_Product_Type]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_Product_Type]
AS
	SELECT UID AS [UID]
	     , Code AS [Code]
	     , Product_Name AS [Product Name] 
		 , Category AS [Category]
		 , Services_Group AS [Services Group]
		 , CASE WHEN (SELECT COUNT(Product_Code) FROM Maintenance_Product WHERE Product_Code = Maintenance_Product_Type.Code AND Customer_ID IN (SELECT Customer_ID FROM Maintenance_Customer WHERE Services_Group = Maintenance_Product_Type.Services_Group)) > 0 THEN 1 ELSE 0 END AS [In Used]
	FROM Maintenance_Product_Type
GO
/****** Object:  View [dbo].[D_Hardkey_Licence_Expired_In_2_Months]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_Hardkey_Licence_Expired_In_2_Months]
AS
	SELECT HL.Customer_ID
	     , C.Name AS [Customer]
		 , C.Country AS [Country]
		 , HL.PO_No AS [PO No]
		 , HL.PO_Date AS [PO Date]
		 , HL.SO_No AS [SO No]
		 , HL.SO_Date AS [SO Date]
		 , HL.Invoice_No AS [Invoice No]
		 , HL.Invoice_Date AS [Invoice Date]
		 , HL.Licence_No AS [Licence No]
		 , HL.PLU_Code AS [PLU Code]
		 , (SELECT Top 1 Value_2 FROM DB_Lookup WHERE Value_1 = HL.PLU_Code) AS [Description]
		 , HL.[Prepared_By] AS [Prepared By]
		 , HL.Created_Date AS [Created Date]
		 , HL.Start_Date AS [Start Date]
		 , HL.End_Date AS [End Date]
		 , (SELECT Top 1 Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = HL.Sales_Representative_ID) AS [Requested By]
	FROM LMS_Hardkey_Licence AS HL 
	INNER JOIN Master_Customer AS C ON C.Customer_ID = HL.Customer_ID
	WHERE DATEDIFF(MONTH, CAST(GETDATE() AS date), HL.End_Date) < 2
GO
/****** Object:  View [dbo].[I_DMC_Subscription]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[I_DMC_Subscription]
AS
SELECT 	*, dbo.Get_Distributor_Name( ISNULL(dbo.Get_Special_Arranged_Bill_Entity([Subscription ID]), CASE WHEN TBL.[By Distributor] = '' THEN [Customer ID] ELSE ( SELECT Customer_ID FROM Master_Customer WHERE Customer_ID = TBL.[By Distributor] ) END ) ) AS [Invoice Bill To]
FROM (
SELECT C.Customer_ID AS [Customer ID]
     , C.Name AS [Customer]
	 , C.By_Distributor AS [By Distributor]
     , D.Headquarter_ID AS [Headquarter ID]
	 , D.Headquarter_Name AS [Headquarter Name]
     , D.Subscription_ID AS [Subscription ID]
	 , D.Start_Date AS [Start Date]
	 , D.End_Date AS [End Date]
	 , R.[Invoice No]
	 , R.[Invoice Date]
	 , COUNT(D.Store_ID) AS [No of Stores] 
	 , SUM(D.Fee) / COUNT(D.Store_ID) AS [Fee Per Store]
	 , R.Currency AS [Currency]
	 , ROUND(SUM(D.Fee), 2) AS [Total Amount]
	 --, CAST(ISNULL(dbo.Get_Licence_Inv_Amount(R.[Invoice No], D.Subscription_ID), 0) + SUM(D.Fee) AS decimal(10, 2)) AS [Total Amount]
	 , R.[Payment Status] AS [Status]
FROM R_DMC_Subscription_Detail D
INNER JOIN R_DMC_Subscription R ON R.[Subscription ID] = D.Subscription_ID
INNER JOIN Master_Customer C ON C.Customer_ID = D.Customer_ID
WHERE R.[Payment Status] IS NULL OR R.[Payment Status] ! = 'Cancelled'
GROUP BY C.Customer_ID, C.Name, C.By_Distributor, D.Headquarter_ID, D.Headquarter_Name, D.Subscription_ID, D.Start_Date, D.End_Date, R.[Invoice No], R.[Invoice Date], R.Currency, R.[Payment Status]
) TBL

GO
/****** Object:  View [dbo].[D_DMC_Subscription_Outstanding_Invoice]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_DMC_Subscription_Outstanding_Invoice]
AS
SELECT [Subscription ID]
     , [Invoice Bill To]
	 , [Headquarter ID]
	 , [Headquarter Name]
	 , [Currency]
	 , [Total Amount]
	 , [Status]
     , CASE WHEN [Invoice No]  = '' THEN 'TBA' ELSE [Invoice No]  END AS [Invoice No] 
FROM I_DMC_Subscription
WHERE [Invoice No]  Is NULL OR [Invoice No]  = '' 

GO
/****** Object:  View [dbo].[D_LMS_Module_Licence_Order_Outstanding_Invoice]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[D_LMS_Module_Licence_Order_Outstanding_Invoice]
AS
SELECT ML.Customer_ID [Customer ID]
     , C.Name AS [Licensee]
	 , ML.PO_No AS [PO No]
	 , ML.PO_Date AS [PO Date]
	 , ML.Invoice_No AS [Invoice No]
	 , ML.Invoice_Date AS [Invoice Date]
	 , ML.Created_Date AS [Created Date]
     , S.Name As [Requested By]
	 , CASE WHEN ML.Chargeable = 1 THEN 'Yes' ELSE 'No' END AS [Chargeable], Remarks, I.[e.Sense], I.BYOC, I.AI, ML.UID 
FROM LMS_Module_Licence_Order ML 
INNER JOIN Master_Customer C ON C.Customer_ID = ML.Customer_ID 
INNER JOIN Master_Sales_Representative S ON S.Sales_Representative_ID = ML.Sales_Representative_ID 
INNER JOIN ( Select * FROM (Select UID, Module_Type, Quantity FROM LMS_Module_Licence_Order_Item ) As SourceTable 
                             PIVOT 
                             (    SUM(Quantity) 
 		                          FOR Module_Type IN ([e.Sense], [BYOC], [AI]) 
	                         ) AS PivotTable ) I ON I.UID = ML.UID 
WHERE (ML.Invoice_No IS NULL OR ML.Invoice_No = '') AND ML.Is_Cancelled = 0
GO
/****** Object:  View [dbo].[R_AI_Licence_Renewal]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_AI_Licence_Renewal]
AS
	SELECT Renewal_UID AS [UID]
	     , Licence_Code AS [Licence Code]
		 , dbo.Get_AI_Licence_Expiry_Date(Licence_Code) AS [Expiry Date]
		 , PO_No AS [PO No]
		 , PO_Date AS [PO Date]
		 , Invoice_No AS [Invoice No]
		 , Invoice_Date AS [Invoice Date]
		 , Renewal_Date AS [Renewal Date]
		 , Chargeable
		 , Currency
		 , Fee
		 , Remarks
		 , Customer_ID AS [Customer ID]
		 , (SELECT Name FROM Master_Customer WHERE Customer_ID = LMS_AI_Licence_Renewal.Customer_ID) AS Customer
		 , Sales_Representative_ID AS [Requestor ID]
		 , (SELECT Name FROM Master_Sales_Representative WHERE Sales_Representative_ID = LMS_AI_Licence_Renewal.Sales_Representative_ID) AS [Requested By]
	FROM LMS_AI_Licence_Renewal
GO
/****** Object:  View [dbo].[I_AI_Licence_Renewal]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[I_AI_Licence_Renewal]
AS
	--SELECT [Customer ID], [Licensee]
	--	 , [Licence Code]
	--	 --, dbo.Get_AI_Licence_Activation_Key([AI Device ID]) AS [Licence Code]
	--	 , [Serial No], [MAC Address], [AI Device ID], [AI Device Serial No]
	--	 , [Activated Date], [Expired Date], [Status]
	--	 , ISNULL((SELECT TOP 1 PO_No FROM LMS_AI_Licence_Renewal WHERE Licence_Code = R_Activated_AI_Licence.[Licence Code] ORDER BY Renewal_UID DESC), (SELECT TOP 1 [PO No] FROM R_LMS_Module_Licence WHERE [Licence Code] = R_Activated_AI_Licence.[Licence Code])) AS [PO No]
	--	 , ISNULL((SELECT TOP 1 PO_Date FROM LMS_AI_Licence_Renewal WHERE Licence_Code = R_Activated_AI_Licence.[Licence Code] ORDER BY Renewal_UID DESC), (SELECT TOP 1 [PO Date] FROM R_LMS_Module_Licence WHERE [Licence Code] = R_Activated_AI_Licence.[Licence Code])) AS [PO Date]
	--	 , ISNULL((SELECT TOP 1 [Requested By] FROM R_AI_Licence_Renewal WHERE [Licence Code] = R_Activated_AI_Licence.[Licence Code] ORDER BY [UID] DESC), (SELECT TOP 1 [Requested By] FROM R_LMS_Module_Licence WHERE [Licence Code] = R_Activated_AI_Licence.[Licence Code])) AS [Requested By]
	--FROM R_Activated_AI_Licence

	WITH RankedResults AS (
		SELECT [Customer ID], Licensee
			 , [Licence Code] 
			 , [Serial No],	[MAC Address], [AI Device ID], [AI Device Serial No], [Activated Date]
			 , CASE WHEN [Expired Date] = 'No Expiry' THEN 'No Expiry' 
					ELSE CAST([Expired Date] AS nvarchar) 
				    END AS [Expired Date]
			 , [Status]
		     , [Requested By]
			 , ROW_NUMBER() OVER (PARTITION BY [Customer ID], [AI Device ID], [MAC Address] ORDER BY [Activated Date] DESC) AS RowNum
		FROM R_Activated_AI_Licence A
	)

	SELECT [Customer ID], Licensee
		 , [Licence Code] 
		 , [Serial No], [MAC Address], [AI Device ID], [AI Device Serial No], [Activated Date], [Expired Date], [Status]
		 , [Requested By]
	FROM RankedResults
	WHERE RowNum = 1;

GO
/****** Object:  Table [dbo].[DB_FileUpload]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_FileUpload](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[File_Name] [nvarchar](200) NOT NULL,
	[Uploaded_DateTime] [nvarchar](100) NOT NULL,
	[Content_Type] [nvarchar](200) NULL,
	[File_Data] [varbinary](max) NULL,
	[Doc_Category] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_File_Repository]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_File_Repository]
AS
	SELECT ID, File_Name, CAST(Uploaded_DateTime AS datetime) AS Uploaded_Date, Content_Type, Doc_Category AS Category
	FROM DB_FileUpload
GO
/****** Object:  Table [dbo].[TempTable_Maintenance_Services_Monthly_Revenue_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable_Maintenance_Services_Monthly_Revenue_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_Maintenance_Services_Revenue_Overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_Maintenance_Services_Revenue_Overview]
AS
SELECT [Year]
     , [COL]
	 , ISNULL([Jan], 0) AS [Jan]
	 , ISNULL([Feb], 0) AS [Feb]
	 , ISNULL([Mar], 0) AS [Mar]
	 , ISNULL([Apr], 0) AS [Apr]
	 , ISNULL([May], 0) AS [May]
	 , ISNULL([Jun], 0) AS [Jun]
	 , ISNULL([Jul], 0) AS [Jul]
	 , ISNULL([Aug], 0) AS [Aug]
	 , ISNULL([Sep], 0) AS [Sep]
	 , ISNULL([Oct], 0) AS [Oct]
	 , ISNULL([Nov], 0) AS [Nov]
	 , ISNULL([Dec], 0) AS [Dec]
	 , CASE WHEN [COL] = 'Amount' THEN (ISNULL([Jan], 0) + ISNULL([Feb], 0) + ISNULL([Mar], 0) + ISNULL([Apr], 0) + ISNULL([May], 0) + ISNULL([Jun], 0) + ISNULL([Jul], 0) + ISNULL([Aug], 0) + ISNULL([Sep], 0) + ISNULL([Oct], 0) + ISNULL([Nov], 0) + ISNULL([Dec], 0)) ELSE 0 END AS Total
FROM (
		SELECT [Year], [Month], COL, VAL FROM TempTable_Maintenance_Services_Monthly_Revenue_Summary
        CROSS APPLY (VALUES('Amount', Total_Amount), ('No of contract', CAST(No_Of_Store AS int)), ('Average', Average)) CS (COL, VAL)) T
        PIVOT (MAX([VAL]) FOR [Month] IN ([Jan], [Feb], [Mar], [Apr], [May], [Jun], [Jul], [Aug], [Sep], [Oct], [Nov], [Dec])) PVT
WHERE YEAR(GETDATE()) - [Year] <= 5
GO
/****** Object:  View [dbo].[_AI_Licence_Notifications_Email_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_AI_Licence_Notifications_Email_List]
AS
		WITH CTE_Sales_Rep AS (
			SELECT Sales_Representative_ID, Short_Name, Email, Supported_By, Supervised_By
			FROM Master_Sales_Representative
		),
		CTE_Support_Rep AS (
			SELECT Sales_Representative_ID, Short_Name AS Supported_Short_Name, Email AS Supported_Email
			FROM Master_Sales_Representative
		),
		CTE_Supervisor_Rep AS (
			SELECT Sales_Representative_ID, Email AS Supervisor_Email
			FROM Master_Sales_Representative
		)

		SELECT A.[Requestor ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + '; ' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM D_Licence_With_Term A
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = A.[Requestor ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) 
		  AND [Application Type] IN ('PC Scale (AI)') 
		  AND [Status] NOT IN ('Renew', 'Blocked', 'Expired') 
		  AND [Customer ID] NOT IN ('CTR-000005')
		  AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') 
		UNION
		SELECT B.[Requestor ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + '; ' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM R_LMS_Module_Licence B 
		INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = B.[Requestor ID]
		LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
		LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By
		WHERE [status] IN ('Renew') 
		  AND [Customer ID] NOT IN ('CTR-000005')
		UNION
		SELECT C.[Requestor ID] AS [Recipient ID]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Short_Name + ', ' + SR_Support.Supported_Short_Name ELSE SR.Short_Name END AS [Recipient Name]
			 , CASE WHEN LEN(SR.Supported_By) > 0 THEN SR.Email + '; ' + SR_Support.Supported_Email ELSE SR.Email END AS [Recipient Email]
			 , COALESCE(SR_Supervisor.Supervisor_Email, '') AS [Cc_Email]
			 , (SELECT TOP 1 Value_2 FROM DB_Lookup WHERE Lookup_Name = 'DMC Cloud Administrator' AND Value_3 = 1) AS [Bcc_Email]
		FROM D_Licence_With_Term C
				INNER JOIN CTE_Sales_Rep SR ON SR.Sales_Representative_ID = C.[Requestor ID]
				LEFT JOIN CTE_Support_Rep SR_Support ON SR_Support.Sales_Representative_ID = SR.Supported_By
				LEFT JOIN CTE_Supervisor_Rep SR_Supervisor ON SR_Supervisor.Sales_Representative_ID = SR.Supervised_By 
		WHERE [Expired Date] <= DATEADD (dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + 3, 0)) 
		  AND [Application Type] IN ('PC Scale (AI)') 
		  AND [Status] IN ('Expired') 
		  AND [Customer ID] NOT IN ('CTR-000005')
		  AND Replace([Licence Code], '-', '') NOT IN (SELECT Replace(Value_1, '-', '') FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key') 


	-- Testing Data
	--SELECT 'S0005' AS [Recipient ID]
	--     , 'Rodney' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0007' AS [Recipient ID]
	--     , 'Henry' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
	--UNION
	--SELECT 'S0010' AS [Recipient ID]
	--     , 'My Tey' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0011' AS [Recipient ID]
	--     , 'Mr Low' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]
 --   UNION
	--SELECT 'S0027' AS [Recipient ID]
	--     , 'Frankie' AS [Recipient Name]
	--	 , 'horace.kang@sg.digi.inc' AS [Recipient Email]
	--	 , '' AS [Cc_Email]
	--	 , '' AS [Bcc_Email]


GO
/****** Object:  Table [dbo].[TempTable_DMC_Monthly_Revenue_Summary]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] [int] NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[R_DMC_Subscription_Revenue_Overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_DMC_Subscription_Revenue_Overview]
AS
SELECT [Year]
     , [COL]
	 , ISNULL([Jan], 0) AS [Jan]
	 , ISNULL([Feb], 0) AS [Feb]
	 , ISNULL([Mar], 0) AS [Mar]
	 , ISNULL([Apr], 0) AS [Apr]
	 , ISNULL([May], 0) AS [May]
	 , ISNULL([Jun], 0) AS [Jun]
	 , ISNULL([Jul], 0) AS [Jul]
	 , ISNULL([Aug], 0) AS [Aug]
	 , ISNULL([Sep], 0) AS [Sep]
	 , ISNULL([Oct], 0) AS [Oct]
	 , ISNULL([Nov], 0) AS [Nov]
	 , ISNULL([Dec], 0) AS [Dec]
	 , CASE WHEN [COL] = 'Amount' THEN (ISNULL([Jan], 0) + ISNULL([Feb], 0) + ISNULL([Mar], 0) + ISNULL([Apr], 0) + ISNULL([May], 0) + ISNULL([Jun], 0) + ISNULL([Jul], 0) + ISNULL([Aug], 0) + ISNULL([Sep], 0) + ISNULL([Oct], 0) + ISNULL([Nov], 0) + ISNULL([Dec], 0)) ELSE 0 END AS Total
FROM (
		SELECT [Year], [Month], COL, VAL FROM Temptable_DMC_Monthly_Revenue_Summary
        CROSS APPLY (VALUES('Amount', Total_Amount), ('No of store', CAST(No_Of_Store AS int)), ('Average', Average)) CS (COL, VAL)) T
        PIVOT (MAX([VAL]) FOR [Month] IN ([Jan], [Feb], [Mar], [Apr], [May], [Jun], [Jul], [Aug], [Sep], [Oct], [Nov], [Dec])) PVT
WHERE YEAR(GETDATE()) - [Year] <= 4
GO
/****** Object:  View [dbo].[R_AI_Licence_CZL_Account_Master]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[R_AI_Licence_CZL_Account_Master]
AS

WITH MasterCustomer AS (
    SELECT Customer_ID, Name, Country
    FROM Master_Customer
),
CZLAccount AS (
    SELECT Client_ID, By_Distributor, User_Group
    FROM CZL_Account
),
LMSLicenseStatus AS (
    SELECT DISTINCT [AI_Device_ID], Synced_dmcmobiletoken_status
    FROM LMS_Licence
),
ExcludedCustomers AS (
    SELECT TRIM(Value_2) AS Customer_ID
    FROM DB_Lookup
    WHERE Lookup_Name = 'Record Excluded' AND Value_1 = 'LMS Profile ID'
),
ActivatedAILicenses AS (
    SELECT [Customer ID], [Licensee], [Serial No], [AI Device Serial No], [AI Device ID], [MAC Address], [Licence Code],
           [Activated Date], [Status], [Expired Date], [Requested By]
    FROM R_Activated_AI_Licence
),
CZLLicensedDevices AS (
    SELECT Scale_SN AS [Serial No], Device_Serial AS [AI Device Serial No], Device_ID AS [AI Device ID], 
           MAC_Addr AS [MAC Address], Client_ID AS [AI Account No], Effective_Date AS [Effective Date]
    FROM CZL_Licenced_Devices
)
SELECT COALESCE(A.[Serial No], B.[Serial No]) AS [Serial No],
       COALESCE(A.[AI Device Serial No], B.[AI Device Serial No]) AS [AI Device Serial],
       COALESCE(A.[AI Device ID], B.[AI Device ID]) AS [AI Device ID],
       COALESCE(A.[MAC Address], B.[MAC Address]) AS [MAC Address],
       A.[Licence Code] AS [Binding Key],
       A.[Customer ID] AS [Customer ID],
       CASE 
           WHEN A.[Licensee] IS NULL THEN MC.Name
           ELSE A.[Licensee] 
       END AS [Licensee],
       A.[Activated Date],
       A.[Expired Date],
       CASE 
           WHEN A.[Status] IS NULL THEN 
               COALESCE((SELECT TOP 1 Synced_dmcmobiletoken_status FROM LMSLicenseStatus WHERE [AI_Device_ID] = COALESCE(A.[AI Device ID], B.[AI Device ID])), 'New')
           ELSE A.[Status]
       END AS [Status],
       CASE 
           WHEN A.[Requested By] IS NULL THEN 
               (SELECT TOP 1 [Requested By] FROM ActivatedAILicenses WHERE [Customer ID] = CA.By_Distributor)
           ELSE A.[Requested By] 
       END AS [Requested By],
       B.[AI Account No],
       CA.User_Group AS [AI Account Name],
       MC.Country AS [Country],
       B.[Effective Date]
FROM ActivatedAILicenses A
FULL OUTER JOIN CZLLicensedDevices B 
    ON A.[Serial No] = B.[Serial No] 
    OR A.[AI Device Serial No] = B.[AI Device Serial No]
    OR A.[AI Device ID] = B.[AI Device ID]
    OR A.[MAC Address] = B.[MAC Address]
LEFT JOIN MasterCustomer MC 
    ON A.[Customer ID] = MC.Customer_ID
LEFT JOIN CZLAccount CA 
    ON B.[AI Account No] = CA.Client_ID
WHERE A.[Customer ID] NOT IN (SELECT Customer_ID FROM ExcludedCustomers);


--SELECT COALESCE(A.[Serial No], B.[Serial No]) AS [Serial No]
--     , COALESCE(A.[AI Device Serial No], B.[AI Device Serial No]) AS [AI Device Serial]
--	 , COALESCE(A.[AI Device ID], B.[AI Device ID]) AS [AI Device ID]
--	 , COALESCE(A.[MAC Address], B.[MAC Address]) AS [MAC Address]
--	 , A.[Licence Code] AS [Binding Key]
--	 , A.[Customer ID] AS [Customer ID]
--	 , CASE WHEN A.[Licensee] IS NULL 
--	        THEN (SELECT Name FROM Master_Customer WHERE Customer_ID = (SELECT By_Distributor FROM CZL_Account WHERE Client_ID = B.[AI Account No])) 
--			ELSE A.[Licensee] 
--			END AS [Licensee]
--	 , A.[Activated Date]
--	 , A.[Expired Date]
--	 , CASE WHEN A.[Status] IS NULL 
--	        THEN CASE WHEN (SELECT TOP 1 Synced_dmcmobiletoken_status FROM LMS_Licence WHERE [AI_Device_ID] = COALESCE(A.[AI Device ID], B.[AI Device ID])) is null then 'New' ELSE (SELECT TOP 1 Synced_dmcmobiletoken_status FROM LMS_Licence WHERE [AI_Device_ID] = COALESCE(A.[AI Device ID], B.[AI Device ID])) END
--		    ELSE A.[Status] END AS [Status]
--	 , CASE WHEN A.[Requested By] IS NULL THEN (SELECT TOP 1 [Requested By] FROM R_Activated_AI_Licence WHERE [Customer ID] = (SELECT By_Distributor FROM CZL_Account WHERE Client_ID = B.[AI Account No])) ELSE A.[Requested By] END AS [Requested By]
--	 , B.[AI Account No]
--	 , (SELECT TOP 1 User_Group FROM CZL_Account WHERE Client_ID = B.[AI Account No]) AS [AI Account Name]
--	 , (SELECT country FROM Master_Customer WHERE Customer_ID = A.[Customer ID]) AS [Country]
--     , B.[Effective Date]
--	 --, A.[Remarks]
--FROM (	SELECT [Customer ID], [Licensee], [Serial No], [AI Device Serial No], [AI Device ID], [MAC Address], [Licence Code], [Activated Date], [Status], [Expired Date], [Requested By], [Remarks]
--		FROM R_Activated_AI_Licence
--	 ) AS A
--FULL OUTER JOIN (	SELECT Scale_SN AS [Serial No], Device_Serial AS [AI Device Serial No], Device_ID AS [AI Device ID], MAC_Addr AS [MAC Address], Client_ID AS [AI Account No], Effective_Date AS [Effective Date]
--					FROM CZL_Licenced_Devices
--				) AS B ON A.[Serial No] = B.[Serial No] 
--					   OR A.[AI Device Serial No] = B.[AI Device Serial No] 
--                       OR A.[AI Device ID] = B.[AI Device ID]
--                       OR A.[MAC Address] = B.[MAC Address]
--WHERE [Customer ID] NOT IN (SELECT TRIM(Value_2) FROM DB_Lookup WHERE Lookup_Name = 'Record Excluded' AND Value_1 = 'LMS Profile ID')
GO
/****** Object:  View [dbo].[Sushiro_China_DMC_Quarterly_Renewal_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Sushiro_China_DMC_Quarterly_Renewal_List]
AS
	SELECT ISNULL(dbo.Get_Distributor_Name(dbo.Get_Special_Arranged_Bill_Entity(TBL.[Subscription ID])), [Bill Entity]) AS [Bill Entity]
		 , [HQ Code], [HQ Name], [Store Code], [Store Name], [Start Date], [End Date], [Duration], [Currency], [Fee], [Status], [Sales Representative]
	FROM (
			SELECT (SELECT TOP 1 Subscription_ID FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Subscription ID]
				 , CASE WHEN C.By_Distributor = '' THEN C.Name ELSE (Select Name From Master_Customer Where Customer_ID = C.By_Distributor) END AS [Bill Entity]
				 , C.Name AS [Customer Name] 
				 , G.Name AS [Group]
				 , H.Headquarter_ID AS [HQ Code]
				 , H.Name AS [HQ Name]
				 , CASE WHEN S.Synced_dmcstore_userstoreid IS NOT NULL THEN Synced_dmcstore_userstoreid ELSE CAST(SUBSTRING(S.Store_ID, 8, 4) As int) END AS [Store Code]
				 , S.Name AS [Store Name]
				 , S.Created_Date AS [Created Date]
				 , (SELECT TOP 1 CONVERT(nvarchar, Start_Date, 23) FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Start Date]
				 , (SELECT TOP 1 CONVERT(nvarchar, End_Date, 23) FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [End Date]
				 , (SELECT TOP 1 Duration FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Duration]
				 , (SELECT TOP 1 Currency FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Currency]
				 , (SELECT TOP 1 Fee FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Fee]
				 , CASE WHEN S.Is_Active = 1 THEN 'Active' ELSE 'In-Active' END AS [Status]
				 , T.Name AS [Account Type]
				 , (SELECT TOP 1 MR.Name 
					FROM DMC_Headquarter_Sales_Representative R
					INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
					WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
					ORDER BY R.Effective_Date DESC) AS [Sales Representative]
			FROM DMC_Store S
			INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
			INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
			INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
			INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
			WHERE S.Account_Type IN ('03') AND S.Is_Active = 1
	) TBL
	WHERE [HQ Code] IN (SELECT Headquarter_ID FROM DMC_Headquarter WHERE [Name] LIKE '%SUSHIRO%') AND [Customer Name] IN (SELECT Name FROM Master_Customer where Country = 'China')
	--AND [End Date] BETWEEN DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()), 0) AND DATEADD(DAY, -1, DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()) + 1, 0))
	--AND [End Date] BETWEEN DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()) + 1, 0) AND DATEADD(DAY, -1, DATEADD(QUARTER, DATEDIFF(QUARTER, 0, GETDATE()) + 2, 0))
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Subscription_By_Customer]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Subscription_By_Customer](@Customer_ID nvarchar(20)) RETURNS TABLE 
AS
RETURN 
(
	SELECT [Subscription ID], ISNULL(dbo.Get_Distributor_Name(dbo.Get_Special_Arranged_Bill_Entity(TBL.[Subscription ID])), [Bill Entity]) AS [Bill Entity]
		 , [Customer Name], [Group], [HQ Code], [HQ Name], [Store Code], [Store Name], [Created Date], [Start Date], [End Date], [Duration], [Currency], [Fee], [Status], [Account Type], [Sales Representative]
	FROM (
					SELECT (SELECT TOP 1 Subscription_ID FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Subscription ID]
						 , CASE WHEN C.By_Distributor = '' THEN C.Name 
								ELSE (Select Name From Master_Customer Where Customer_ID = C.By_Distributor) END AS [Bill Entity]
						 , C.Name AS [Customer Name] 
						 , G.Name AS [Group]
						 , H.Headquarter_ID AS [HQ Code]
						 , H.Name AS [HQ Name]
						 , CASE WHEN S.Synced_dmcstore_userstoreid IS NOT NULL THEN CAST(Synced_dmcstore_userstoreid AS int) ELSE CAST(SUBSTRING(S.Store_ID, 8, 4) As int) END AS [Store Code]
						 , S.Name AS [Store Name]
						 , S.Created_Date AS [Created Date]
						 , (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Start Date]
						 , (SELECT TOP 1 End_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [End Date]
						 , (SELECT TOP 1 Duration FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Duration]
						 , (SELECT TOP 1 Currency FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Currency]
						 , (SELECT TOP 1 Fee FROM DMC_Subscription WHERE Store_ID = S.Store_ID AND Payment_Status NOT IN ('Cancelled') ORDER BY End_Date DESC) AS [Fee]
						 , CASE WHEN S.Is_Active = 1 THEN 'Active' ELSE 'In-Active' END AS [Status]
						 , T.Name AS [Account Type]
						 , (SELECT TOP 1 MR.Name 
							FROM DMC_Headquarter_Sales_Representative R
							INNER JOIN Master_Sales_Representative MR ON MR.Sales_Representative_ID = R.Sales_Representative_ID
							WHERE R.Headquarter_ID = H.Headquarter_ID AND Effective_Date <= (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Store_ID = S.Store_ID ORDER BY End_Date DESC)
							ORDER BY R.Effective_Date DESC) AS [Sales Representative]
					FROM DMC_Store S
					INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = S.Headquarter_ID
					INNER JOIN Master_Account_Type T ON T.Code = S.Account_Type
					INNER JOIN Master_Customer C ON C.Customer_ID = H.Customer_ID
					INNER JOIN Master_Customer_Group G ON G.Group_ID = C.Group_ID
					WHERE S.Account_Type IN ('03') AND S.Is_Active = 1
	) TBL
	WHERE [HQ Code] IN (SELECT Headquarter_ID FROM DMC_Headquarter WHERE Customer_ID = @Customer_ID)
)
GO
/****** Object:  View [dbo].[_Server_Space_Quarter_Moving_Average]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Server_Space_Quarter_Moving_Average]
AS
	WITH CTE AS (
					SELECT A.[Start Date]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Quarter A
					LEFT JOIN _Server_Space_Quarter B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Start Date]
	) 

SELECT [Start Date]
	 , [Used]
	 , [Used Growth]
	 , CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [Space Growth Moving Average]
	 --, CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , [DB Size]
	 , [DB Growth]
	 , CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [DB Growth Moving Average]
	 --, CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [DB Growth Moving Average]
FROM CTE;

GO
/****** Object:  View [dbo].[_Server_Space_Semiannual_Moving_Average]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[_Server_Space_Semiannual_Moving_Average]
AS
	WITH CTE AS (
					SELECT A.[Start Date]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Semiannual A
					LEFT JOIN _Server_Space_Semiannual B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Start Date]
	)

SELECT [Start Date]
	 , [Used]
	 , [Used Growth]
	 , CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [Space Growth Moving Average]
	 --, CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , [DB Size]
	 , [DB Growth]
	 , CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [DB Growth Moving Average]
	 --, CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [DB Growth Moving Average]
FROM CTE;

GO
/****** Object:  Table [dbo].[DB_Exchange_History]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Exchange_History](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Date] [nvarchar](50) NOT NULL,
	[Currency] [nvarchar](50) NOT NULL,
	[Rate] [numeric](10, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[DB_Exchange_History_Base_USD]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[DB_Exchange_History_Base_USD]
AS
	SELECT A.Date, A.Currency, CAST((1/A.rate) * B.Rate AS decimal(10,6)) AS Rate
	FROM DB_Exchange_History A
	INNER JOIN (SELECT Date, Rate FROM DB_Exchange_History WHERE Currency = 'USD') B ON B.Date = A.Date
	WHERE A.Currency = 'EUR'
	UNION
	SELECT Date, 'SGD', CAST((1/Rate) AS decimal(10,6)) AS Rate
	FROM DB_Exchange_History 
	WHERE Currency = 'USD'
GO
/****** Object:  View [dbo].[_Server_Space_Week_Moving_Average]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Week_Moving_Average]
AS
	WITH CTE AS (
					SELECT A.[Week Start] AS [Start Date]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Week A
					LEFT JOIN _Server_Space_Week B ON B.[Week End] = DATEADD(DAY, -1, A.[Week Start])
					GROUP BY A.[Week Start]
	)

SELECT [Start Date]
	 , [Used]
	 , [Used Growth]
	 --, CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , [DB Size]
	 , [DB Growth]
	 --, CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [DB Growth Moving Average]
	 , CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING) AS decimal(10,2)) AS [DB Growth Moving Average]
FROM CTE;

GO
/****** Object:  Table [dbo].[Resource_Consumption]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Resource_Consumption](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Reading_Date] [date] NULL,
	[HQID] [nvarchar](6) NULL,
	[EJournal_Size_MB] [int] NULL,
	[PLU_Count] [int] NULL,
	[Transaction_Total_Count] [int] NULL,
	[Transaction_Item_Count] [int] NULL,
	[Billed_Store] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  View [dbo].[vw_Resource_Consumption]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_Resource_Consumption]
AS
SELECT CTR.Customer_Name
	 , (SELECT Name FROM Master_Customer_Group WHERE Group_ID = CTR.Group_ID) AS [Group]
     , CTR.Headquarter_ID
	 , CTR.Headquarter_Name
	 , R.Reading_Date
	 , R.EJournal_Size_MB
	 , R.PLU_Count
	 , R.Transaction_Total_Count
	 , R.Transaction_Item_Count
	 , R.Billed_Store
FROM Resource_Consumption AS R 
RIGHT JOIN (
				SELECT C.Name AS Customer_Name, Group_ID, Headquarter_ID, H.Name AS Headquarter_Name 
				FROM Master_Customer C
				INNER JOIN DMC_Headquarter H ON H.Customer_ID = C.Customer_ID
		   ) AS CTR ON CTR.Headquarter_ID = R.HQID
GO
/****** Object:  View [dbo].[_Server_Space_Year_Moving_Average]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[_Server_Space_Year_Moving_Average]
AS
	WITH CTE AS (
					SELECT A.[Start Date]
						 , MAX(A.[Server Space]) AS [Server Space]
						 , MAX(A.[Used]) AS [Used]
						 , ISNULL(MAX(A.[Used]) - MAX(B.[Used]), 0) AS [Used Growth]
						 , MAX(A.[DB Size]) AS [DB Size]
						 , ISNULL(MAX(A.[DB Size]) - MAX(B.[DB Size]), 0) AS [DB Growth]
					FROM _Server_Space_Year A
					LEFT JOIN _Server_Space_Year B ON B.[End Date] = DATEADD(DAY, -1, A.[Start Date])
					GROUP BY A.[Start Date]
	)

SELECT [Start Date]
	 , [Used]
	 , [Used Growth]
	 , CAST(AVG([Used Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [Space Growth Moving Average]
	 , [DB Size]
	 , [DB Growth]
	 , CAST(AVG([DB Growth]) OVER (ORDER BY [Start Date] ROWS BETWEEN 2 PRECEDING AND CURRENT ROW) AS decimal(10,2)) AS [DB Growth Moving Average]
FROM CTE;

GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_Base_USD]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_Base_USD](@ReportMonth date) RETURNS TABLE 
AS
RETURN 
(
	-- @ReportMonth return end date of previous month
	SELECT Subscription_ID
		 , Customer_ID
		 , Customer
		 , Country
		 , Device_Type
	     , Headquarter_ID
	     , Headquarter_Name
	     , Store_No
	     , Store_Name
		 , Created_Date
	     , Start_Date
	     , End_Date
	     , dbo.Get_NumberOfMonth(Start_Date, End_Date) AS No_Of_Month
	     , CASE WHEN DATEDIFF(Month, @ReportMonth, End_Date) <= dbo.Get_NumberOfMonth(Start_Date, End_Date)
	            THEN CASE WHEN DATEDIFF(Month, @ReportMonth, End_Date) > 0 THEN DATEDIFF(Month, @ReportMonth, End_Date) ELSE 0 END 
			    ELSE dbo.Get_NumberOfMonth(Start_Date, End_Date) END AS Remaining_Cycle
	     , CASE WHEN Currency != 'USD' THEN 'USD' ELSE Currency END AS Currency
	     , CAST((dbo.Monthly_Avg_Exchange_Rate_Base_USD(@ReportMonth, Currency) * Fee) / dbo.Get_NumberOfMonth(Start_Date, End_Date) AS decimal(10, 2)) AS Monthly_Fee
    FROM R_DMC_Subscription_Detail
    WHERE Start_Date <= @ReportMonth AND End_Date >= @ReportMonth
)
GO
/****** Object:  View [dbo].[vw_Resource_Consumption_Overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_Resource_Consumption_Overview]
AS
SELECT YEAR([Reading Date]) AS [Year]
     , FORMAT([Reading Date], 'MMM') AS [Month]
	 , [Reading Date]
     , [ColName], [Reading Value] 
FROM (
		SELECT EOMONTH(CAST([Year] AS nvarchar) + '-' + CAST([Month] AS nvarchar) + '-' + '01') AS [Reading Date]
		     , [ColName], [Reading Value]
		FROM (
				-- Start of Unpivot View 
				SELECT TBL1.[Year]
                     , TBL1.[Month]
	                 , TBL2.EJournal AS [EJournal Size]
	                 , TBL2.PLU AS [PLU Count]
	                 , TBL2.Transaction_Total AS [Transaction Total]
	                 , TBL2.Transaction_Item AS [Transaction Item]
					 , TBL2.Billed_Store AS [Billed Store]
	                 , (SELECT Used_Percentage FROM Server_Space WHERE Reading_Date = MaxDate) AS [Disk Space Used]
					 , (SELECT DB_Size FROM Server_Space WHERE Reading_Date = MaxDate) AS [DB Size]  
                FROM (
		               SELECT YEAR(Reading_Date) AS [Year]
			                , MONTH(Reading_Date) AS [Month]
			                , MAX(Reading_Date) AS MaxDate
		               FROM Server_Space
		               GROUP BY YEAR(Reading_Date), MONTH(Reading_Date)
                     ) TBL1 
                INNER JOIN (
				              SELECT YEAR(Reading_Date) AS [Year]
					               , MONTH(Reading_Date) AS [Month]
					               , SUM(ISNULL(EJournal_Size_MB, 0)) AS EJournal
					               , SUM(ISNULL(PLU_Count, 0)) AS PLU
	                               , SUM(ISNULL(Transaction_Total_Count, 0)) AS Transaction_Total
					               , SUM(ISNULL(Transaction_Item_Count, 0)) AS Transaction_Item
								   , SUM(ISNULL(Billed_Store, 0)) AS Billed_Store  
				              FROM vw_Resource_Consumption
				              WHERE Reading_Date IS NOT NULL 
							    --AND Reading_Date = EOMONTH(DATEADD(m, -1,GETDATE()))
				                --AND YEAR(reading_date) IS NOT NULL
				              GROUP BY YEAR(Reading_Date), MONTH(Reading_Date)
                           ) TBL2 ON TBL2.[Year] = TBL1.[Year] AND TBL2.[Month] = TBL1.[Month]
		          -- End of unpivot view
			   ) T
		CROSS APPLY (
						VALUES ('EJournal Size', [EJournal Size])
						     , ('PLU Count', [PLU Count])
							 , ('Transaction Total', [Transaction Total])
						     , ('Transaction Item', [Transaction Item])
							 , ('Billed Store', [Billed Store])
							 , ('DB Size', [DB Size])
						     , ('Disk Space Used', [Disk Space Used])
					) C([ColName], [Reading Value])
     ) D
GO
/****** Object:  UserDefinedFunction [dbo].[DMC_Monthly_Subscription_By_Account_Type_Base_USD]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[DMC_Monthly_Subscription_By_Account_Type_Base_USD](@ReportMonth date) RETURNS TABLE 
AS
RETURN 
(
	SELECT Device_Type
	     , Country
		 , Headquarter_ID
		 , CASE WHEN Headquarter_Name LIKE '%HQ' THEN Customer ELSE Headquarter_Name END AS Headquarter_Name
		 , CAST(COUNT(Store_Name) AS int) AS Owned_Store
		 , Currency
		 , CAST(SUM(Monthly_Fee) AS decimal(10, 2)) AS Total_Amount_Per_Month
		 , CAST(SUM(Monthly_Fee) / COUNT(Store_Name) AS decimal(10, 2)) AS Average
	FROM dbo.DMC_Monthly_Subscription_Base_USD(@ReportMonth)
	GROUP BY Device_Type, Country, Headquarter_ID, Customer, Headquarter_Name, Currency
)
GO
/****** Object:  View [dbo].[L_dmcstore]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_dmcstore]
AS
	-- This is a view created from dmclive dmcstore table
	SELECT  *
	FROM    OPENQUERY(DMCLIVE, 'Select hqid, id, name, updates, ftpuser, ftppass, userstoreid, saleslastuseddate from dmcstore') AS derived_dmcstore

GO
/****** Object:  View [dbo].[vw_Current_Expired_Store]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[vw_Current_Expired_Store]
AS
	SELECT E.hqid, E.id AS storeid, E.name, E.updates 
		 , R.[End Date], R.[Account Type], R.[Status], R.[Store ID], R.[Account Type] + ' period expired' AS Reason
	FROM L_dmcstore E
	INNER JOIN R_DMC_Store_Licence R ON CAST(R.[Headquarter ID] AS int) = E.hqid AND E.id = R.Synced_dmcstore_id
	WHERE R.Status = 'Active' AND CAST(R.[End Date] AS date) < DATEADD(d, -1, GETDATE())
GO
/****** Object:  UserDefinedFunction [dbo].[Get_Financial_Year_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_Financial_Year_List]() 
RETURNS TABLE 
AS
RETURN 
(
	WITH YearList AS (
                       SELECT YEAR(GETDATE()) AS Year
                       UNION ALL
                       SELECT Year - 1
                       FROM YearList
                       WHERE Year > YEAR(GETDATE()) - 6
                     )
    SELECT Year FROM YearList
)
GO
/****** Object:  UserDefinedFunction [dbo].[Get_MonthYearList]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[Get_MonthYearList] (
	@StartDate date, 
	@EndDate date) 
RETURNS TABLE 
AS
RETURN 
(
	WITH DateMonth AS (
							SELECT CONVERT(datetime, @StartDate) AS MonthYearList
							UNION ALL
							SELECT DATEADD(MM, 1, MonthYearList) 
							FROM DateMonth 
							WHERE MonthYearList < CONVERT(datetime, EOMONTH(@EndDate, - 1))
                      )

	SELECT LEFT(DATENAME(MM, MonthYearList), 3) + ' ' + CONVERT(nvarchar, YEAR(MonthYearList)) MonthYearList 
	FROM DateMonth
)
GO
/****** Object:  View [dbo].[DB_List_Of_Year]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DB_List_Of_Year]
AS
WITH YearList AS 
(
    SELECT 2000 AS [YEAR]
    UNION ALL
    SELECT yl.[YEAR] + 1 AS [YEAR]
    FROM YearList yl
    WHERE yl.[YEAR] + 1 <= YEAR(GETDATE())
)
SELECT [YEAR] FROM YearList
GO
/****** Object:  View [dbo].[L_Archiving_Tables_overview]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[L_Archiving_Tables_overview]
AS
	-- This view retrieve module licence pool from DMC
	SELECT Table_Name
	     , CAST( CASE WHEN Table_Size LIKE '%MB' THEN CAST(REPLACE(Table_Size, ' MB', '') AS FLOAT) * 1024 -- Convert MB to KB
                      WHEN Table_Size LIKE '%GB' THEN CAST(REPLACE(Table_Size, ' GB', '') AS FLOAT) * 1024 * 1024 -- Convert GB to KB
                      WHEN Table_Size LIKE '%bytes' THEN 0 -- Treat '0 bytes' as 0
                      ELSE 0 END AS INT) AS Table_Size_KB
	FROM OPENQUERY(DMCLIVE, 'SELECT table_name, pg_size_pretty(pg_relation_size(quote_ident(table_name))) AS table_size
                             FROM information_schema.tables
                             WHERE table_schema = ''public''
                               AND table_name IN (SELECT DISTINCT SUBSTR(table_name, 1, LENGTH(table_name) - 4) FROM information_schema.tables WHERE table_name LIKE ''%20%'')
                             UNION
                             SELECT table_name, pg_size_pretty(pg_relation_size(quote_ident(table_name)))
                             FROM information_schema.tables
                             WHERE table_schema = ''public'' 
                               AND table_name IN (SELECT table_name FROM information_schema.tables WHERE table_name LIKE ''%20%'')
                             ORDER BY 1') AS derived_table
GO
/****** Object:  View [dbo].[L_bloated_table_monitoring]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_bloated_table_monitoring]
AS
	-- To retrieve bloated table information
	SELECT  table_name AS [Table Name]
	      , total_size AS [Table Size]
		  , data_size AS [Data Size]
		  , bloat_size AS [Bloated Size]
		  , ROUND(Threshold, 1) AS [Ratio]
	FROM    OPENQUERY(DMCLIVE, 'SELECT tablename AS table_name
                                     , pg_size_pretty(pg_total_relation_size(schemaname || ''.'' || tablename)) AS total_size
                                     , pg_size_pretty(pg_table_size(schemaname || ''.'' || tablename)) AS data_size
                                     , pg_size_pretty(pg_total_relation_size(schemaname || ''.'' || tablename) - pg_table_size(schemaname || ''.'' || tablename)) AS bloat_size
                                     , (pg_total_relation_size(schemaname || ''.'' || tablename)::double precision / pg_table_size(schemaname || ''.'' || tablename)::double precision) AS Threshold
                                FROM pg_tables
                                WHERE pg_total_relation_size(schemaname || ''.''|| tablename) > (1.2 * pg_table_size(schemaname || ''.'' || tablename)) -- Example threshold, adjust as needed
                                  AND pg_table_size(schemaname || ''.'' || tablename) > 100000000
								  AND schemaname = ''public''
                                ORDER BY (pg_total_relation_size(schemaname || ''.'' || tablename) - pg_table_size(schemaname || ''.'' || tablename)) DESC;') AS bloated_table_information
GO
/****** Object:  View [dbo].[L_bloated_table_monitoring_percentage]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_bloated_table_monitoring_percentage]
AS
	-- To retrieve bloated table information
	SELECT  table_name AS [Table Name]
	      , total_size AS [Table Size In Bytes]
		  , bloat_bytes AS [Bloated Size In Bytes]
		  , ROUND(bloat_percent, 2) AS [Bloated Percentage]
	FROM    OPENQUERY(DMCLIVE, 'WITH bloat_stats AS (
														SELECT nspname AS schemaname
															 , relname AS table_name
                                                             , reltuples AS row_estimate
                                                             , pg_total_relation_size(c.oid) AS total_bytes
                                                             , pg_relation_size(c.oid) AS table_bytes
                                                             , pg_total_relation_size(c.oid) - pg_relation_size(c.oid) AS bloat_bytes
                                                             , CASE WHEN pg_total_relation_size(c.oid) > 0 
                                                                    THEN (pg_total_relation_size(c.oid) - pg_relation_size(c.oid))::numeric / pg_total_relation_size(c.oid)
                                                                    ELSE 0
                                                                    END AS bloat_ratio
                                                        FROM pg_class c
                                                        JOIN pg_namespace n ON c.relnamespace = n.oid
                                                        WHERE relkind = ''r''
                                                          AND nspname NOT IN (''pg_catalog'', ''information_schema'')
                                                    )
                                SELECT table_name
                                     , total_bytes AS total_size
                                     , bloat_bytes
                                     , (bloat_ratio * 100)::float AS bloat_percent
                                FROM bloat_stats
                                WHERE total_bytes > 0  
                                  AND total_bytes > bloat_bytes 
                                  AND bloat_bytes > 100000000
                                  AND bloat_ratio > 0.30
                                ORDER BY bloat_bytes DESC') AS bloated_table_information

	--SELECT  table_name AS [Table Name]
	--      , total_size AS [Table Size In Bytes]
	--	  , bloat_bytes AS [Bloated Size In Bytes]
	--	  , ROUND(bloat_percent, 2) AS [Bloated Percentage]
	--FROM    OPENQUERY(DMCLIVE, 'WITH bloat_stats AS (
	--													SELECT nspname AS schemaname
	--														 , relname AS table_name
 --                                                            , reltuples AS row_estimate
GO
/****** Object:  View [dbo].[L_database_size]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[L_database_size]
AS
	-- To retrieve database size from DMC
	SELECT  *
	FROM    OPENQUERY(DMCLIVE, 'SELECT pg_size_pretty(pg_database_size(''dmc''))') AS database_size
GO
/****** Object:  View [dbo].[L_dbflexpayment]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[L_dbflexpayment]
AS
	-- This is a view created from dmclive table
	SELECT  hqid, storeid, name, id, type, paymenttype, updates
	FROM    OPENQUERY(DMCLIVE, 'Select hqid, storeid, name, id, type, paymenttype, updates from dbflexpayment Where updates = 1') AS derived_dbflexpayment
GO
/****** Object:  View [dbo].[L_dbtax]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[L_dbtax]
AS
	-- This is a view created from dmclive dmclive table
	SELECT  hqid, storeid, name, id, rate, updates
	FROM    OPENQUERY(DMCLIVE, 'Select hqid, storeid, name, id, rate, updates from dbtax Where updates = 1') AS derived_dbtax
GO
/****** Object:  View [dbo].[L_dmcitemtax]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[L_dmcitemtax]
AS
	-- This is a view created from dmclive dmclive table
	SELECT  hqid, storeid, name, id, rate, updates
	FROM    OPENQUERY(DMCLIVE, 'Select hqid, storeid, name, id, rate, updates from dmcitemtax Where updates = 1') AS derived_dmcitemtax
GO
/****** Object:  View [dbo].[L_dmclicensetoken]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_dmclicensetoken]
AS
	-- This is a view created from dmclive dmcmobiletoken table
	SELECT  licensecode, token, createdate, activatedate, CAST(expireddate AS date) AS expireddate
	FROM    OPENQUERY(DMCLIVE, 'Select licensecode, token, createdate, activatedate, to_timestamp(expireddate::text, ''YYYYMMDD'') AS expireddate from dmclicensetoken Where updates = 1') AS derived_dmclicensetoken
GO
/****** Object:  View [dbo].[L_dmcmobiletoken]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[L_dmcmobiletoken]
AS
	-- This is a view created from dmclive dmcmobiletoken table
	SELECT  id, hqid, storeid, license_code, status, createdate, activateddate, expireddate, app_type, updates, email, unique_id, term, maxhq, maxstore, maxresetdate, totalreset
	FROM    OPENQUERY(DMCLIVE, 'Select id, hqid, storeid, license_code, status, createdate, activateddate, expireddate, app_type, updates, email, unique_id, term, maxhq, maxstore, maxresetdate, totalreset from dmcmobiletoken Where updates = 1') AS derived_dmcmobiletoken
GO
/****** Object:  View [dbo].[L_dmcuser]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[L_dmcuser]
AS
	-- This is a view created from dmclive dmcuser table
	SELECT  username, email, devicetype, hqid
	FROM    OPENQUERY(DMCLIVE, 'Select username, email, devicetype, hqid from dmcuser Where updates = 1 and username not in (''admin'')') AS derived_dmcuser
GO
/****** Object:  Table [dbo].[CZL_Account_Notes]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CZL_Account_Notes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CZL_Account_Unique_ID] [nvarchar](20) NULL,
	[Notes] [nvarchar](max) NULL,
	[Added_Date] [date] NULL,
	[Notes_For] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Access_Map]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Access_Map](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Icon] [nvarchar](200) NULL,
	[Module_Name] [nvarchar](100) NOT NULL,
	[Level_1] [int] NOT NULL,
	[Sub_Module] [nvarchar](100) NULL,
	[Level_2] [int] NOT NULL,
	[Description] [nvarchar](100) NULL,
	[Path] [nvarchar](400) NULL,
	[Admin] [bit] NULL,
	[Tech] [bit] NULL,
	[Sales] [bit] NULL,
	[CtrAdmin] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Account_Notes]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Account_Notes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Notes] [nvarchar](max) NULL,
	[Added_Date] [date] NULL,
	[Notes_For] [nvarchar](30) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_AuditLog]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_AuditLog](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Event_Name] [nvarchar](100) NULL,
	[Event_Date] [nvarchar](50) NULL,
	[Username] [nvarchar](100) NULL,
	[Role] [nvarchar](50) NULL,
	[Client] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Bill_Entity_Special_Arranged]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Bill_Entity_Special_Arranged](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Subscription_ID] [nvarchar](20) NULL,
	[Arranged_Bill_Entity] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Knowledge_Base]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Knowledge_Base](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Article_No] [nvarchar](50) NULL,
	[Article_Title] [nvarchar](300) NULL,
	[Article_Category] [nvarchar](100) NULL,
	[Article_Content] [nvarchar](max) NULL,
	[Submitted_On] [datetime] NULL,
	[Submitted_By] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Reminder]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Reminder](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Created_Date] [date] NOT NULL,
	[Reminder] [nvarchar](500) NULL,
	[Completed_Date] [date] NULL,
	[Is_Done] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Smtp]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Smtp](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Host] [nvarchar](100) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[Port] [int] NOT NULL,
	[SSL_Enabled] [bit] NOT NULL,
	[Is_Valid] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DB_Users]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DB_Users](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](250) NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Roles] [nvarchar](50) NOT NULL,
	[Display_Name] [nvarchar](100) NULL,
	[User_Group] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Account_Reports_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Account_Reports_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Report_Name] [nvarchar](100) NULL,
	[Page_Origin] [nvarchar](max) NULL,
	[Path] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DMC_Subscription_Staging]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DMC_Subscription_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Store_ID] [nvarchar](20) NULL,
	[Synced_dmcstore_userstoreid] [nvarchar](10) NULL,
	[Duration] [nvarchar](20) NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Payment_Method] [nvarchar](20) NULL,
	[Payment_Mode] [nvarchar](20) NULL,
	[Subscriber_Group] [nvarchar](1) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Excel_Reports_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Excel_Reports_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Report_Name] [nvarchar](100) NULL,
	[Description] [nvarchar](300) NULL,
	[SQLString] [nvarchar](max) NULL,
	[SheetName] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FTP_Server_Distributor]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FTP_Server_Distributor](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Distributor] [nvarchar](100) NOT NULL,
	[Country] [nvarchar](100) NULL,
	[Currency] [nvarchar](3) NULL,
	[COY_ABBR] [nvarchar](3) NULL,
	[Region] [nvarchar](50) NULL,
	[TS_Rep] [nvarchar](50) NULL,
	[MKT_Rep] [nvarchar](50) NULL,
	[Nature] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FTP_Server_Distributor_Access_List]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FTP_Server_Distributor_Access_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Access_List_UID] [nvarchar](30) NOT NULL,
	[User_ID] [nvarchar](10) NOT NULL,
	[Code] [nvarchar](5) NOT NULL,
	[Path] [nvarchar](100) NULL,
	[Folder] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_AI_Licence_Renewal_Staging]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_AI_Licence_Renewal_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Renewal_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Remarks] [nvarchar](100) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_Licence_Reset_History]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Licence_Reset_History](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[MAC_Address] [nvarchar](max) NULL,
	[Counter] [int] NOT NULL,
	[Reset_On] [datetime] NULL,
	[Remarks] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_Licence_Staging]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Licence_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[PO_No] [nvarchar](50) NULL,
	[Application_Type] [nvarchar](30) NULL,
	[OS_Type] [nvarchar](30) NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[Email] [nvarchar](100) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
	[Chargeable] [bit] NULL,
	[Remarks] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_Module_Licence_Staging]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Module_Licence_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[PO_No] [nvarchar](50) NULL,
	[Module_Type] [nvarchar](20) NULL,
	[Quantity] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LMS_Termed_Licence_Renewal_Staging]    Script Date: 13/6/2025 8:49:43 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LMS_Termed_Licence_Renewal_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Licence_Code] [nvarchar](50) NULL,
	[PO_No] [nvarchar](50) NULL,
	[PO_Date] [date] NULL,
	[Invoice_No] [nvarchar](30) NULL,
	[Invoice_Date] [date] NULL,
	[Renewal_Date] [date] NULL,
	[Chargeable] [bit] NULL,
	[Currency] [nvarchar](5) NULL,
	[Fee] [money] NULL,
	[Remarks] [nvarchar](100) NULL,
	[Customer_ID] [nvarchar](20) NULL,
	[Sales_Representative_ID] [nvarchar](10) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Contract_Details]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Contract_Details](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Contract_Unique_ID] [nvarchar](20) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Product_Unique_ID] [nvarchar](20) NOT NULL,
	[Serial_No] [nvarchar](20) NOT NULL,
	[Product_Name] [nvarchar](100) NOT NULL,
	[Base_Currency] [nvarchar](3) NOT NULL,
	[Base_Currency_Value] [money] NOT NULL,
	[Maintenance_Cost] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Contract_Product_Line_Items_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Contract_Product_Line_Items_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Product_Unique_ID] [nvarchar](20) NOT NULL,
	[Serial_No] [nvarchar](20) NOT NULL,
	[Product_Name] [nvarchar](100) NOT NULL,
	[Base_Currency] [nvarchar](3) NOT NULL,
	[Base_Currency_Value] [money] NOT NULL,
	[Maintenance_Cost] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_ESL_Tags_Deployment_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_ESL_Tags_Deployment_Staging](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Customer_ID] [nvarchar](20) NOT NULL,
	[Store_ID] [nvarchar](20) NOT NULL,
	[Tags_Group] [nvarchar](20) NOT NULL,
	[Tags_Type] [nvarchar](10) NOT NULL,
	[Quantity] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Maintenance_Services_Reports_List]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Maintenance_Services_Reports_List](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Report_Name] [nvarchar](100) NULL,
	[Page_Origin] [nvarchar](max) NULL,
	[Path] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TempTable]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TempTable](
	[HQID] [nvarchar](10) NULL,
	[Reading_Value] [int] NULL
) ON [PRIMARY]
GO
/****** Object:  StoredProcedure [dbo].[Display_Consumption_Data]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Display_Consumption_Data] 
	@NoOfMonth int

AS
DECLARE @idx int
SET @idx = 1

DECLARE @MonthCols nvarchar(max)
SET @MonthCols = '[' + SUBSTRING(DATENAME(Month, DATEADD(m, @idx * -1, GETDATE())), 1, 3) + 
                 ' ' + SUBSTRING(DATENAME(yy, DATEADD(m, @idx * -1, GETDATE())), 3, 2) + ']'

DECLARE @DisplayCols nvarchar(max)
SET @DisplayCols = ''

DECLARE @sqlCommand nvarchar(max)
SET @sqlCommand = ( ' SELECT TBL.[ColName], ISNULL(TBL.[Reading Value], 0) AS ' + @MonthCols + ' <DisplayCols>' +
                    ' FROM ( ' +
		            '          SELECT ColName, Month, [Reading Value] ' + 
		            '          FROM vw_Resource_Consumption_Overview ' +
		            '          WHERE MONTH([Reading Date]) = MONTH(DATEADD(m, ' + CAST(@idx * -1 AS nvarchar) + ', GETDATE())) ' +
					'            AND YEAR([Reading Date]) = YEAR(DATEADD(m, ' + CAST(@idx * -1 AS nvarchar) + ', GETDATE())) ' +
                    '      ) TBL ' )

WHILE @idx <= @NoOfMonth
BEGIN
	SET @MonthCols = '[' + SUBSTRING(DATENAME(Month, DATEADD(m, (@idx + 1) * -1, GETDATE())), 1, 3) + 
                 ' ' + SUBSTRING(DATENAME(yy, DATEADD(m, (@idx + 1) * -1, GETDATE())), 3, 2) + ']'
				 		
	SET @DisplayCols += ', ISNULL(TBL' + CAST(@idx AS nvarchar) + '.[Reading Value], 0) AS ' + @MonthCols

	SET @sqlCommand += ' LEFT JOIN ( ' +
			           '             SELECT ColName, Month, [Reading value] ' + 
			           '             FROM vw_Resource_Consumption_Overview ' +
			           '             WHERE MONTH([Reading Date]) = MONTH(DATEADD(m, ' + CAST((@idx + 1) * -1 AS nvarchar) + ', GETDATE())) ' +
					   '               AND YEAR([Reading Date]) = YEAR(DATEADD(m, ' + CAST((@idx + 1) * -1 AS nvarchar) + ', GETDATE())) ' +
                       '           ) TBL' + CAST(@idx AS nvarchar) + ' ON TBL' + CAST(@idx AS nvarchar) + '.ColName = TBL.colName'

	SET @idx = @idx + 1		
END

SET @sqlCommand = REPLACE(@sqlcommand, '<DisplayCols>', @DisplayCols)
EXECUTE(@sqlCommand)

	
GO
/****** Object:  StoredProcedure [dbo].[Insert_Consumption_Data]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Insert_Consumption_Data] 
	@filename nvarchar(100)

AS
DECLARE @HQID int
DECLARE @Value int

DECLARE @Billed_Store int

DECLARE record_cursor CURSOR
FOR
	SELECT HQID, Reading_Value FROM TempTable
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @HQID, @Value


WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @filename = 'dbtransactionitem.csv'
			BEGIN
				IF EXISTS( SELECT * FROM Resource_Consumption 
				           WHERE HQID = @HQID 
						     AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
						 )
					BEGIN
						UPDATE Resource_Consumption
						SET Transaction_Item_Count = @Value
						WHERE HQID = @HQID
							AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
					END
				ELSE
					BEGIN
						INSERT INTO Resource_Consumption(Reading_Date, HQID, Transaction_Item_Count)
						SELECT EOMONTH(DATEADD(m,-1, GETDATE())), FORMAT(CAST(HQID AS int), 'd6'), Reading_Value 
						FROM TempTable
						WHERE HQID = @HQID
					END					
			END
		ELSE IF @filename = 'dbtransactiontotal.csv'
			BEGIN
				IF EXISTS( SELECT * FROM Resource_Consumption 
				           WHERE HQID = @HQID 
						     AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
						 )
					BEGIN
						UPDATE Resource_Consumption
						SET Transaction_Total_Count = @Value
						WHERE HQID = @HQID
						    AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
					END
				ELSE
					BEGIN
						INSERT INTO Resource_Consumption(Reading_Date, HQID, Transaction_Total_Count)
						SELECT EOMONTH(DATEADD(m,-1, GETDATE())), FORMAT(CAST(HQID AS int), 'd6'), Reading_Value 
						FROM TempTable
						WHERE HQID = @HQID
					END					
			END
		ELSE IF @filename = 'ejournalsize.csv'
			BEGIN
				IF EXISTS( SELECT * FROM Resource_Consumption 
				           WHERE HQID = @HQID 
						     AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
						 )
					BEGIN
						UPDATE Resource_Consumption
						SET EJournal_Size_MB = @Value
						WHERE HQID = @HQID
							AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
					END
				ELSE
					BEGIN
						INSERT INTO Resource_Consumption(Reading_Date, HQID, EJournal_Size_MB)
						SELECT EOMONTH(DATEADD(m,-1, GETDATE())), FORMAT(CAST(HQID AS int), 'd6'), Reading_Value 
						FROM TempTable
						WHERE HQID = @HQID
					END					
			END
		ELSE IF @filename = 'plu.csv'
			BEGIN
				IF EXISTS( SELECT * FROM Resource_Consumption 
				           WHERE HQID = @HQID 
						     AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
						 )
					BEGIN
						UPDATE Resource_Consumption
						SET PLU_Count = @Value
						WHERE HQID = @HQID
						    AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
					END
				ELSE
					BEGIN
						INSERT INTO Resource_Consumption(Reading_Date, HQID, PLU_Count)
						SELECT EOMONTH(DATEADD(m,-1, GETDATE())), FORMAT(CAST(HQID AS int), 'd6'), Reading_Value 
						FROM TempTable
						WHERE HQID = @HQID
					END														
			END
		
		-- Insert Billed Store data
		BEGIN
			SET @Billed_Store = ( SELECT COUNT(*) FROM DMC_Store 
			                      WHERE SUBSTRING(Store_ID, 2, 6) = @HQID AND Account_Type = '03' )
			                       
			IF EXISTS( SELECT * FROM Resource_Consumption 
				       WHERE HQID = @HQID 
					     AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
					 )
				BEGIN
						UPDATE Resource_Consumption
						SET Billed_Store = @Billed_Store
						WHERE HQID = @HQID
						    AND Reading_Date = EOMONTH(DATEADD(m, -1, GETDATE()))
				END
			ELSE
				BEGIN
						INSERT INTO Resource_Consumption(Reading_Date, HQID, Billed_Store)
						SELECT EOMONTH(DATEADD(m,-1, GETDATE())), FORMAT(CAST(HQID AS int), 'd6'), @Billed_Store 
						FROM TempTable
						WHERE HQID = @HQID
				END
		END


		FETCH NEXT FROM record_cursor INTO @HQID, @Value
	END

CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[Read_Consumption_Data]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[Read_Consumption_Data] 
	@foldername nvarchar(100)

AS
DECLARE @DirCommandPath nvarchar(200)
SET @DirCommandPath = 'dir D:\' + @foldername + ' /b'

DECLARE @files TABLE (ID int IDENTITY, fname varchar(100))
INSERT INTO @files EXECUTE xp_cmdshell @DirCommandPath

DECLARE @FileName nvarchar(100)
DECLARE @FilePath nvarchar(200)
DECLARE @CSVImportCommand nvarchar(max)


DECLARE record_cursor1 CURSOR
FOR
	SELECT fname FROM @files WHERE fname IS NOT NULL
OPEN record_cursor1
FETCH NEXT FROM record_cursor1 INTO @FileName


WHILE @@FETCH_STATUS = 0
	BEGIN	

			-- Create TempTable
			DROP TABLE IF EXISTS dbo.TempTable
			CREATE TABLE TempTable (
				[HQID] nvarchar(10) NULL,
				[Reading_Value] int NULL
			) ON [PRIMARY]

			-- Import flatfile into soucejournal table
			SET @FilePath = (SELECT 'D:\' + @foldername + '\' + @filename + '')
			SET @CSVImportCommand = ('BULK INSERT dbo.TempTable ' +  
			                         'FROM ''' + @filepath + '''' +
						             'WITH ' +
			                         '( ' +
			                         '	FIRSTROW = 1, ' +
							         '	FIELDTERMINATOR = '','', ' +
							         '	ROWTERMINATOR = ''0x0a'', ' +    
							         '	TABLOCK, ' +
							         '    CODEPAGE = ''ACP'' );'
							         )
			-- Execute import command
			EXEC(@CSVImportCommand)

			-- Run Store Procedured to insert or update record to Resource_Consumption table
			EXEC Insert_Consumption_Data @FileName

			FETCH NEXT FROM record_cursor1 INTO @Filename
	END

CLOSE record_cursor1
DEALLOCATE record_cursor1

GO
/****** Object:  StoredProcedure [dbo].[SP_AuditLog]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_AuditLog]
		@Event_Name nvarchar(100),
		@Event_Date nvarchar(50),
		@Username nvarchar(100),
		@Role nvarchar(50),
		@Client nvarchar(50)

AS

BEGIN

	-- Capture login user into auditlog table
	INSERT INTO DB_AuditLog(Event_Name, Event_Date, Username, Role, Client)
	VALUES(@Event_Name, @Event_Date, @Username, @Role, @Client)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Auto_Reminder]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Auto_Reminder]
AS
DECLARE @SSLEndDate As date
DECLARE @DMCUpgradeDate As date
BEGIN
	-- 01. Reminder to prepare for monthly server resource consumption data 5 days before the month end
	IF DATEDIFF(d, GETDATE(), EOMONTH(GETDATE())) = 5
		BEGIN
			INSERT INTO DB_Reminder(Created_Date, Reminder, Completed_Date, Is_Done)
			SELECT GETDATE(), 'Monthly - Prepare server resource consumption data for ' + DATENAME(MONTH, GETDATE()) + ' ' + DATENAME(YEAR, GETDATE()), NULL, 0 
		END

	-- 02. Reminder for SSL Renewal
	SET @SSLEndDate = (SELECT TOP 1 Value_3 FROM DB_Lookup WHERE Lookup_Name like '%SSL Certificate%' ORDER BY Value_3 DESC)
	IF DATEDIFF(d, GETDATE(), @SSLEndDate) = 30 AND @SSLEndDate IS NOT NULL
		BEGIN
			INSERT INTO DB_Reminder(Created_Date, Reminder, Completed_Date, Is_Done)
			SELECT GETDATE(), 'Renewal of SSL Certificate which end on ' + FORMAT(@SSLEndDate, 'dd MMM yyyy'), NULL, 0
		END

	-- 03. Reminder for DMC Software upgrade
	SET @DMCUpgradeDate = (SELECT TOP 1 Maintenance_Date FROM DMC_Maintenance_History WHERE Work_Type = 'Software upgrade' AND Status = 'Scheduled' ORDER BY Maintenance_Date DESC)
	IF DATEDIFF(d, GETDATE(), @DMCUpgradeDate) = 30 AND @DMCUpgradeDate IS NOT NULL
		BEGIN
			INSERT INTO DB_Reminder(Created_Date, Reminder, Completed_Date, Is_Done)
			SELECT GETDATE(), 'Prepare all notifications for DMC Upgrade on ' + FORMAT(@DMCUpgradeDate, 'dd MMM yyyy'), NULL, 0

			INSERT INTO DB_Reminder(Created_Date, Reminder, Completed_Date, Is_Done)
			SELECT GETDATE(), 'To turn off RC Server on ' + FORMAT(DATEADD(DAY, -1, @DMCUpgradeDate), 'dd MMM yyyy'), NULL, 0
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Change_Order_PO_No]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Change_Order_PO_No]
	@Customer_ID nvarchar(20),
	@Category nvarchar(30),
	@Old_PO_No nvarchar(50),
	@New_PO_No nvarchar(50)
AS
BEGIN
	IF @Category = 'Hardkey Licence'
		BEGIN
			UPDATE LMS_Hardkey_Licence 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No
		END
	ELSE IF @Category = 'App Product Licence'
		BEGIN 
			UPDATE LMS_Licence 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Application_Type NOT IN ('PC Scale') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No
		END
	ELSE IF @Category = 'Module Licence'
		BEGIN
			UPDATE LMS_Licence 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Application_Type IN ('PC Scale') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No

			UPDATE LMS_Module_Licence_Order 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No						
		END
	ELSE IF @Category = 'Termed Licence Renewal'
		BEGIN
			UPDATE LMS_Termed_Licence_Renewal 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No				
		END
	ELSE IF @Category = 'AI Licence Renewal'
		BEGIN
			UPDATE LMS_AI_Licence_Renewal 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND Customer_ID = @Customer_ID AND PO_No = @Old_PO_No				
		END
	ELSE IF @Category = 'CZL Account Setup'
		BEGIN
			UPDATE CZL_Account_Setup_Charge 
			SET PO_No = @New_PO_No
			WHERE PO_No NOT IN ('NA') AND CZL_Account_Unique_ID IN (SELECT CZL_Account_Unique_ID FROM CZL_Account WHERE By_Distributor = @Customer_ID) AND PO_No = @Old_PO_No				
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Create_Article]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Create_Article]
		@Article_Title nvarchar(300),
		@Article_Category nvarchar(100),
		@Article_Content nvarchar(max),
		@Submitted_By nvarchar(100)
AS
DECLARE @Article_No nvarchar(50)

BEGIN
	SET @Article_No = CASE WHEN EXISTS(select Article_No from db_Knowledge_Base) 
						   THEN (SELECT TOP 1 'ART-' + FORMAT(SUBSTRING(Article_No, 6, 8) + 1, 'd8') FROM db_Knowledge_Base ORDER BY Article_No DESC)
						   ELSE ('ART-00000001')
						   END 

	-- Create an article record
	INSERT INTO DB_Knowledge_Base(Article_No, Article_Title, Article_Category, Article_Content, Submitted_On, Submitted_By)
	VALUES(@Article_No, @Article_Title, @Article_Category, @Article_Content, GETDATE(), @Submitted_By)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Create_Excel_Report]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Create_Excel_Report]
		@ReportName nvarchar(100),
		@Description nvarchar(300),
		@SheetName nvarchar(max),
		@SqlStrings nvarchar(max)

AS 
BEGIN

/* Insert records */
INSERT INTO Excel_Reports_List(Report_Name, Description, SQLString, SheetName)
VALUES(@ReportName, @Description, @SqlStrings, @SheetName)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_AI_Licence_Renewal]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_AI_Licence_Renewal] 
	@PO_No nvarchar(50),
	@Customer_ID nvarchar(20)

AS
DECLARE @AILicenceRenewalID nvarchar(20)
SET @AILicenceRenewalID = (SELECT dbo.Get_New_AI_Renewal_ID())

BEGIN
	IF EXISTS(SELECT * FROM LMS_AI_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No)
	BEGIN
		INSERT INTO LMS_AI_Licence_Renewal(Renewal_UID, Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID)
		SELECT @AILicenceRenewalID, Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID FROM LMS_AI_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
	END

	-- Insert record to DB_SO_No_By_PO table
	IF NOT EXISTS(SELECT * FROM DB_SO_No_By_PO WHERE PO_No = @PO_No)
	BEGIN
		INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date)
		SELECT TOP 1 Customer_ID, Sales_Representative_ID, PO_No, PO_Date FROM LMS_AI_Licence_Renewal WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Renewal_UID = @AILicenceRenewalID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Customer]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Customer]
		@Customer_ID nvarchar(20),
		@Name nvarchar(100),
		@Distributor_Code nvarchar(10),
		@Country nvarchar(50),
		@Address nvarchar(255),
		@Type nvarchar(20),
		@Group_ID nvarchar(10),
		@By_Distributor nvarchar(20),
		@Contact_Person nvarchar(50),
		@Phone nvarchar(50),
		@Email nvarchar(100),
		@BtnCommand nvarchar(10)
AS
DECLARE @NextCustomer_ID nvarchar(20)

BEGIN
	IF @BtnCommand = 'Create'
		BEGIN
			SET @NextCustomer_ID = dbo.Get_New_Customer_ID()
			INSERT INTO Master_Customer(Customer_ID, Name, Type, Address, Country, By_Distributor, Group_ID, Distributor_Code, Created_Date, Is_Active, Inactive_Date, Contact_Person, Phone, Email)
			VALUES(@NextCustomer_ID, @Name, @Type, @Address, @Country, @By_Distributor, @Group_ID, @Distributor_Code, GETDATE(), 1, NULL, @Contact_Person, @Phone, @Email)
		END
	ELSE IF @BtnCommand = 'Update'
		BEGIN 
			UPDATE Master_Customer
			SET Name = @Name
			  , Type = @Type
			  , Address = @Address
			  , Country = @Country
			  , By_Distributor = @By_Distributor
			  , Group_ID = @Group_ID
			  , Distributor_Code = @Distributor_Code
			  , Contact_Person = @Contact_Person
			  , Phone = @Phone
			  , Email = @Email
			WHERE Customer_ID = @Customer_ID
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Account]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Account]
		@CZL_Account_Unique_ID nvarchar(20),
		@Client_ID nvarchar(5),
		@User_Group nvarchar(50),
		@Account_Model nvarchar(10),
		@By_Distributor nvarchar(20),
		@Country nvarchar(50),
		@Gen_Version nvarchar(20),
		@Effective_Date date,
		@BtnCommand nvarchar(10)
AS
DECLARE @Next_CZL_Account_Unique_ID nvarchar(20)

BEGIN
	IF @BtnCommand = 'Create'
		BEGIN
			SET @Next_CZL_Account_Unique_ID = dbo.Get_New_CZL_Account_Unique_ID()
			INSERT INTO CZL_Account(CZL_Account_Unique_ID, Client_ID, Created_Date, User_Group, By_Distributor, Country, Gen_Version, Effective_Date, One_Year_Period_End, Account_Model)
			VALUES(@Next_CZL_Account_Unique_ID, @Client_ID, GETDATE(), @User_Group, @By_Distributor, @Country, @Gen_Version, @Effective_Date, CAST(DATEADD(DAY, -1, DATEADD(YEAR, 1, @Effective_Date)) AS date), @Account_Model)
		END
	ELSE IF @BtnCommand = 'Update'
		BEGIN 
			UPDATE CZL_Account
			SET Client_ID = @Client_ID
			  , User_Group = @User_Group
			  , By_Distributor = @By_Distributor
			  , Country = @Country
			  , Gen_Version = @Gen_Version
			  , Account_Model = @Account_Model
			WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID
		END
END


GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Account_Notes]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Account_Notes]
		@CZL_Account_Unique_ID nvarchar(20),
		@Notes nvarchar(max),
		@Notes_For nvarchar(30),
		@ID int
AS
BEGIN
IF NOT EXISTS(SELECT * FROM CZL_Account_Notes WHERE ID = @ID)
	BEGIN
		INSERT INTO CZL_Account_Notes(CZL_Account_Unique_ID, Notes, Added_Date, Notes_For)
		VALUES(@CZL_Account_Unique_ID, @Notes, GETDATE(), @Notes_For)
	END
ELSE
	BEGIN
		UPDATE CZL_Account_Notes
		SET Notes = @Notes
		WHERE ID = @ID
	END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Account_Setup_Charge]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Account_Setup_Charge]
		@CZL_Account_Unique_ID nvarchar(20),
		@Client_ID nvarchar(5),
		@PO_No nvarchar(50),
		@PO_Date date,
		@Currency nvarchar(5),
		@Fee money,
		@Sales_Representative_ID nvarchar(10)
AS
DECLARE @Current_CZL_Account_Unique_ID nvarchar(20)
SET @Current_CZL_Account_Unique_ID = (SELECT TOP 1 CZL_Account_Unique_ID FROM CZL_Account ORDER BY ID DESC)
BEGIN
	IF NOT EXISTS(SELECT * FROM CZL_Account_Setup_Charge WHERE CZL_Account_Unique_ID = @Current_CZL_Account_Unique_ID)
	BEGIN
		INSERT INTO CZL_Account_Setup_Charge(CZL_Account_Unique_ID, Client_ID, PO_No, PO_Date, Invoice_No, Invoice_Date, Currency, Fee, Sales_Representative_ID)
		VALUES(@Current_CZL_Account_Unique_ID, @Client_ID, CAST(@PO_No AS nvarchar(50)), @PO_Date, '', NULL, @Currency, @Fee, @Sales_Representative_ID)
	END

	-- Insert record to DB_SO_No_By_PO table
	IF NOT EXISTS(SELECT * FROM DB_SO_No_By_PO WHERE PO_No = @PO_No)
	BEGIN
		INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date)
		SELECT TOP 1 [Distributor ID], [Requestor ID], CAST([PO No] AS nvarchar(50)), [PO Date] FROM I_CZL_Account_Setup_Fee WHERE [PO No] = @PO_No
	END
END


GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Licenced_Devices]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Licenced_Devices]
		@Device_Serial nvarchar(20),
		@Device_ID nvarchar(100), 
		@Model nvarchar(10),
		@AI_Software_Version nvarchar(10),
		@R_Version nvarchar(10),
		@Scale_SN nvarchar(50),
		@MAC_Addr nvarchar(50),
		@Production_Licence_No nvarchar(50),
		@Location nvarchar(100),
		@CZL_Client_ID nvarchar(5),
		@Effective_Date date,
		@Unique_ID nvarchar(20),
		@CZL_Account_Unique_ID nvarchar(20)
AS
BEGIN
DECLARE @Client_ID nvarchar(5)  -- An ID that is given by CZL
SET @Client_ID = (SELECT Client_ID FROM CZL_Account WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID)

IF NOT EXISTS(SELECT * FROM CZL_Licenced_Devices WHERE Unique_ID = @Unique_ID)
	BEGIN
		INSERT INTO CZL_Licenced_Devices(Unique_ID, Device_Serial, Device_ID, Model, AI_Software_Version, R_Version, Scale_SN, MAC_Addr, Production_Licence_No, Location, Created_Date, Last_Updated, Effective_date, Client_ID, CZL_Account_Unique_ID)
		VALUES(dbo.Get_New_CZL_Licenced_Device_Unique_ID(), @Device_Serial, @Device_ID, @Model, @AI_Software_Version, @R_Version, @Scale_SN, @MAC_Addr, @Production_Licence_No, @Location, GETDATE(), GETDATE(), @Effective_Date, @Client_ID, @CZL_Account_Unique_ID)
	END
ELSE 
	BEGIN
		UPDATE CZL_Licenced_Devices
		SET Device_Serial = @Device_Serial, 
		    Device_ID = @Device_ID, 
			Model = @Model,
			AI_Software_Version = @AI_Software_Version,
			R_Version = @R_Version,
			Scale_SN = @Scale_SN,
			MAC_Addr = @MAC_Addr,
			Production_Licence_No = @Production_Licence_No,
			Location = @Location,
			Last_Updated = GETDATE(),
			Client_ID = @CZL_Client_ID,
			Effective_date = @Effective_Date,
			CZL_Account_Unique_ID = (SELECT CZL_Account_Unique_ID FROM CZL_Account WHERE Client_ID = @CZL_Client_ID)
		WHERE Unique_ID = @Unique_ID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Log]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Log]
        @ID int,
		@Log nvarchar(max),
		@Unique_ID nvarchar(20),
		@Log_Type nvarchar(3),
		@By_Who nvarchar(50)
AS
BEGIN
IF NOT EXISTS(SELECT * FROM CZL_Device_Status_Log WHERE ID = @ID)
	BEGIN
		INSERT INTO CZL_Device_Status_Log(Created_Date, Log_Description, Unique_ID, Log_Type, Last_Update, By_Who)
		VALUES(GETDATE(), @Log, @Unique_ID, @Log_Type, GETDATE(), @By_Who)
	END
ELSE
	BEGIN
		UPDATE CZL_Device_Status_Log
		SET Log_Description = @Log,
		    Log_Type = @Log_Type,
			Last_Update = GETDATE(),
			By_Who = @By_Who
		WHERE ID = @ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Log_Batch]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Log_Batch]
		@Log nvarchar(max),
		@CZL_Account_Unique_ID nvarchar(20),
		@Log_Type nvarchar(3),
		@By_Who nvarchar(50)
AS
DECLARE @Unique_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR 
	-- Get all the Device Unique ID of a CZL Account to write log in batch
	SELECT Unique_ID FROM CZL_Licenced_Devices 
	WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID
	ORDER BY ID

OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Unique_ID

WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO CZL_Device_Status_Log(Created_Date, Log_Description, Unique_ID, Log_Type, Last_Update, By_Who)
		VALUES(GETDATE(), @Log, @Unique_ID, @Log_Type, GETDATE(), @By_Who)

	FETCH NEXT FROM record_cursor INTO @Unique_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Model_Update]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Model_Update]
		@CZL_Account_Unique_ID nvarchar(20),
		@New_Model nvarchar(10),
		@Effective_Date date,
		@Bind_Key nvarchar(50),
		@Remarks nvarchar(100),
		@By_Who nvarchar(50)
AS
DECLARE @Default_Model nvarchar(10)
DECLARE @Old_Model nvarchar(10)
DECLARE @UID nvarchar(20)

SET @Default_Model = (SELECT TOP 1 Account_Model FROM CZL_Account WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID ORDER BY CAST(ISNULL(Account_Model, 0) AS int) DESC)
SET @Old_Model = ISNULL((SELECT TOP 1 To_Model FROM CZL_Account_Model_Upgrade_Trail WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID ORDER BY Effective_Date desc, To_Model DESC), @Default_Model)
SET @UID = 'UID_' +  FORMAT(GETDATE(), 'yyyyMMddhhmmss')

BEGIN
    -- 01. Add model change record to CZL_Account_Model_Upgrade_Trail table
	INSERT INTO CZL_Account_Model_Upgrade_Trail(CZL_Account_Unique_ID, From_Model, To_Model, Effective_Date, Bind_Key, UID, Last_Update, Remarks)
	VALUES(@CZL_Account_Unique_ID, @Old_Model, @New_Model, @Effective_Date, @Bind_Key, @UID, GETDATE(), @Remarks)

	-- 02. Update CZL_Account - Account_Model column to latest model
	UPDATE CZL_Account SET Account_Model = @New_Model WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID

	-- 03. Get the device prev model and form the log description
	DECLARE @Log nvarchar(max)
	SET @Log = 'Upgraded from model-' + @Old_Model + ' to model-' + @New_Model

	-- 04. Add model change log to CZL_Account_Notes table
	INSERT INTO CZL_Account_Notes(CZL_Account_Unique_ID, Notes, Added_Date, Notes_For)
	VALUES(@CZL_Account_Unique_ID, @Log, GETDATE(), 'CZL Account')
END

DECLARE @Unique_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR 
	-- Get all the Device Unique ID of a CZL Account to write log in batch
	SELECT Unique_ID FROM CZL_Licenced_Devices 
	WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID
	ORDER BY ID

OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Unique_ID

WHILE @@FETCH_STATUS = 0
BEGIN
		-- 05. Update model of all account underlying licensed device
		UPDATE CZL_Licenced_Devices
		SET Model = @New_Model
		WHERE Unique_ID = @Unique_ID   

	FETCH NEXT FROM record_cursor INTO @Unique_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_CZL_Model_Update_Charge]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_CZL_Model_Update_Charge]
		@CZL_Account_Unique_ID nvarchar(20),
		@Client_ID nvarchar(5),
		@Upgraded_Model nvarchar(10),
		@PO_No nvarchar(50),
		@PO_Date date,
		@Bind_Key nvarchar(50)
AS
DECLARE @Distributor_ID nvarchar(20)
DECLARE @Requested_By nvarchar(100)
DECLARE @UID nvarchar(20)

SET @Distributor_ID = (SELECT TOP 1 By_Distributor FROM CZL_Account WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID)
SET @Requested_By = (SELECT TOP 1 [Requested By] FROM R_Headquarter_Sales_Representative WHERE [Customer ID] = @Distributor_ID)
SET @UID = (SELECT TOP 1 UID FROM CZL_Account_Model_Upgrade_Trail ORDER BY ID DESC)

BEGIN
	IF NOT EXISTS(SELECT * FROM CZL_Account_Model_Upgrade_Charge WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID AND Client_ID = @Client_ID AND Upgraded_Model = @Upgraded_Model)
	BEGIN
		INSERT INTO CZL_Account_Model_Upgrade_Charge(CZL_Account_Unique_ID, Client_ID, Upgraded_Model, Upgraded_Date, PO_No, PO_Date, Distributor_ID, Requested_By, Bind_Key, UID)
		VALUES(@CZL_Account_Unique_ID, @Client_ID, @Upgraded_Model, GETDATE(), @PO_No, @PO_Date, @Distributor_ID, @Requested_By, @Bind_Key, @UID)
	END
END


GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_DMC_Headquarter]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_DMC_Headquarter]
		@Headquarter_ID nvarchar(20),
		@Name nvarchar(100), 
		@Customer_ID nvarchar(20),
		@Status nvarchar(1),
		@Sales_Representative_ID nvarchar(10)	
AS
BEGIN
IF EXISTS(SELECT * FROM DMC_Headquarter WHERE Headquarter_ID = @Headquarter_ID)
	BEGIN
		UPDATE DMC_Headquarter
		SET Name = @Name
		  , Is_Active = @Status
		  , Inactive_Date = CASE WHEN @Status = 0 THEN GETDATE() ELSE NULL END
		WHERE Customer_ID = @Customer_ID AND Headquarter_ID = @Headquarter_ID
			
		UPDATE DMC_Headquarter_Sales_Representative
		SET Sales_Representative_ID = @Sales_Representative_ID
		WHERE Headquarter_ID = @Headquarter_ID 
	END
ELSE
	BEGIN
		INSERT INTO DMC_Headquarter(Headquarter_ID, Name, Created_Date, Is_Active, Inactive_Date, Customer_ID)
		VALUES(@Headquarter_ID, @Name, GETDATE(), @Status, NULL, @Customer_ID)

		INSERT INTO DMC_Headquarter_Sales_Representative(Headquarter_ID, Sales_Representative_ID, Effective_Date)
		VALUES(@Headquarter_ID, @Sales_Representative_ID, GETDATE())
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_DMC_Store]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_DMC_Store]
	@Store_No nvarchar(4),
	@Store_Name nvarchar(100),
	@Banner nvarchar(100),
	@Zone nvarchar(50),
	@Account_Type nvarchar(4),
	@Public_IP nvarchar(20),
	@FTP_User nvarchar(20),
	@FTP_Password nvarchar(20),
	@Is_Active int,
	@End_Date nvarchar(20),
	@Headquarter_ID nvarchar(20),
	@Store_ID nvarchar(50)

AS
--DECLARE @Store_ID nvarchar(50)
DECLARE @ExpiryOn date
DECLARE @MonthAdd int

BEGIN
	-- Set the expiry date
	SET @MonthAdd = CASE WHEN DAY(GETDATE()) <= 15 THEN 3 ELSE 4 END

	-- If it is Trial account then set trial end date, else set NULL
	SET @ExpiryOn = CASE WHEN @Account_Type = '01' 
						 THEN CASE WHEN Trim(@End_Date) = '' THEN DATEADD(dd, -1, DATEADD(mm, DATEDIFF(mm, 0, GETDATE()) + @MonthAdd, 0)) ELSE @End_Date END
						 ELSE NULL END

	IF EXISTS (SELECT * FROM DMC_Store WHERE Store_ID = @Store_ID)
		BEGIN				
			UPDATE DMC_Store
			SET Name = @Store_Name
			  , Banner = @Banner
			  , Zone = @Zone
			  , Account_Type = @Account_Type
			  , Public_IP = @Public_IP
			  , FTP_User = @FTP_User
			  , FTP_Password = @FTP_Password
			  , Is_Active = @Is_Active
			  , Inactive_Date = @ExpiryOn 
			  , Last_Updated = GETDATE()
			WHERE Store_ID = @Store_ID	
		END
	ELSE	
		BEGIN
			-- Form new Store_ID
			SET @Store_ID = ('S' + FORMAT(CAST(@Headquarter_ID As int), 'd6') + FORMAT(CAST(@Store_No As int), 'd4'))

			INSERT INTO DMC_Store(Store_ID, Name, Banner, Zone, Account_Type, Created_Date, Effective_Date, Public_IP, FTP_Host, FTP_User, FTP_Password, Is_Active, Inactive_Date, Last_Updated, Headquarter_ID)
			VALUES(@Store_ID, @Store_Name, @Banner, @Zone, @Account_Type, GETDATE(), GETDATE(), @Public_IP, 'dmc.teraoka.com.sg', @FTP_User, @FTP_Password, @Is_Active, @ExpiryOn, GETDATE(), @Headquarter_ID)

			-- Update Synced_dmcstore_id from DMC dmcstore table
			UPDATE DMC_Store
	        SET Synced_dmcstore_id = L_dmcstore.id
			  , Synced_dmcstore_userstoreid = FORMAT(CAST(L_dmcstore.userstoreid As int), 'd4')
	        FROM DMC_Store
	        LEFT JOIN L_dmcstore on L_dmcstore.hqid = CAST(DMC_Store.Headquarter_ID as int) AND L_dmcstore.ftpuser = DMC_Store.FTP_User AND L_dmcstore.ftppass = DMC_Store.FTP_Password 
	        WHERE DMC_Store.Store_ID = @Store_ID 
			  AND L_dmcstore.hqid IS NOT NULL 
			  AND DMC_Store.Synced_dmcstore_id IS NULL
			  AND DMC_Store.Synced_dmcstore_userstoreid IS NULL
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_DMC_Subscription]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_DMC_Subscription]
    @UID nvarchar(20),
	@Customer_ID nvarchar(20)

AS
DECLARE @Store_ID nvarchar(20)
DECLARE @Subscription_ID nvarchar(20)
DECLARE @ExistingStartDate date
DECLARE @NextStartDate date

DECLARE record_cursor CURSOR
FOR 
	SELECT Store_ID
	FROM DMC_Subscription_Staging 
	WHERE SUBSTRING(Store_ID, 2, 6) IN (SELECT Headquarter_ID FROM DMC_Headquarter WHERE Customer_ID = @Customer_ID)
	ORDER BY Headquarter_ID

OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Store_ID

SET @Subscription_ID = CASE WHEN LEN(@UID) > 0 THEN @UID ELSE (SELECT dbo.Get_New_Subscription_ID()) END
SET @ExistingStartDate = (SELECT TOP 1 Start_Date FROM DMC_Subscription WHERE Subscription_ID = @UID)

-- Delete subscription record and ready to import from staging table
DELETE FROM DMC_Subscription WHERE Subscription_ID = @UID

WHILE @@FETCH_STATUS = 0
BEGIN 
	-- Set the Store subscription start date
	SET @NextStartDate = CASE WHEN @ExistingStartDate IS NOT NULL THEN @ExistingStartDate ELSE dbo.Get_New_Subscription_Start_Date(@Store_ID) END

	/** Insert subscription record to DMC_Subscription table **/
	INSERT INTO DMC_Subscription(Subscription_ID, Start_Date, End_Date, Duration, Currency, Fee, Payment_Method, Payment_Mode, Payment_Status, Subscriber_Group, Store_ID)
	SELECT @Subscription_ID
		 , @NextStartDate
		 , DATEADD(DAY, -1, (DATEADD(MONTH, CAST(Duration AS int), @NextStartDate)))
		 , CASE WHEN CAST(Duration AS int) >= 12 THEN CAST(Duration / 12 AS nvarchar) + ' Year(s)' ELSE Duration + ' Month(s)' END
		 , Currency
 		 , ROUND(CAST(Fee AS money), 2)
		 , Payment_Method
		 , Payment_Mode
		 , 'Pending'
		 , Subscriber_Group
		 , @Store_ID
	FROM DMC_Subscription_Staging
	WHERE Store_ID = @Store_ID
	
	FETCH NEXT FROM record_cursor INTO @Store_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor


GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_DMC_Subscription_Batch]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_DMC_Subscription_Batch]
	@Headquarter_ID nvarchar(20),
	@Duration int,
	@Currency nvarchar(5),
	@Fee money,
	@Payment_Method nvarchar(20),
	@Payment_Mode nvarchar(20),
	@Subscriber_Group nvarchar(1)

AS
BEGIN
		WITH Active_Billed_Stores AS (
			SELECT Store_ID FROM DMC_Store
			WHERE Headquarter_ID = @Headquarter_ID
			  AND Account_Type = '03'
			  AND Is_Active = 1
		),
		Subscription_Check AS (
			SELECT Store_ID
			FROM DMC_Subscription
			WHERE SUBSTRING(Store_ID, 2, 6) = @Headquarter_ID
			  AND Store_ID IN (SELECT Store_ID FROM Active_Billed_Stores)
		)
		INSERT INTO DMC_Subscription(Subscription_ID, Start_Date, End_Date, Duration, Currency, Fee, Payment_Method, Payment_Mode, Ref_Invoice_No, Invoiced_Date, Payment_Status, Subscriber_Group, Store_ID)
		SELECT dbo.Get_New_Subscription_ID()
			 , dbo.Get_New_Subscription_Start_Date(Store_ID)
			 , DATEADD(DAY, -1, DATEADD(MONTH, CAST(@Duration AS int), dbo.Get_New_Subscription_Start_Date(Store_ID)))
			 , CASE WHEN CAST(@Duration AS int) >= 12 THEN CAST(@Duration / 12 AS nvarchar) + ' Year(s)' ELSE CAST(@Duration AS nvarchar) + ' Month(s)' END
			 , @Currency, @Fee, @Payment_Method, @Payment_Mode, NULL, NULL, 'Pending', @Subscriber_Group, Store_ID
		FROM (
				SELECT Store_ID FROM Subscription_Check
				UNION
				SELECT Store_ID FROM Active_Billed_Stores
				WHERE Store_ID NOT IN (SELECT Store_ID FROM Subscription_Check)
			  ) AS Stores
		ORDER BY Store_ID
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_DMC_User]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_DMC_User]
	@Username nvarchar(20),
	@Password nvarchar(20),
	@Email nvarchar(100),
	@Device_Type int,
	@Is_Active int,
	@End_Date nvarchar(20),
	@Headquarter_ID nvarchar(20)

AS
DECLARE @ExpiryOn date

BEGIN
SET @ExpiryOn = CASE WHEN Trim(@End_Date) = '' THEN NULL ELSE @End_Date END

IF EXISTS(SELECT * FROM DMC_User WHERE Username = @Username)
BEGIN
	UPDATE DMC_User
	SET Password = @Password
		, Email = @Email
		, Synced_dmcuser_devicetype = @Device_Type
		, Inactive_Date = @ExpiryOn
		, Is_Active = CASE WHEN @ExpiryOn IS NULL THEN 1 ELSE CASE WHEN @ExpiryOn > GETDATE() - 1 THEN 1 ELSE 0 END END 
	WHERE Username = @Username AND Headquarter_ID = @Headquarter_ID
END
ELSE
BEGIN
	INSERT INTO DMC_User(Username, Password, Email, Created_Date, Effective_Date, Is_Active, Inactive_Date, Synced_dmcuser_devicetype, Headquarter_ID)
	VALUES(@Username, @Password, @Email, GETDATE(), GETDATE(), 1, @ExpiryOn, @Device_Type, @Headquarter_ID)
END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_FTP_Account_Users]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_FTP_Account_Users]
		@Code nvarchar(5), 
		@Distributor nvarchar(100),
		@Country nvarchar(100),
		@Currency nvarchar(3),
		@COY_ABBR nvarchar(3),
		@Region nvarchar(50),
		@TS_Rep nvarchar(50),
		@MKT_Rep nvarchar(50),
		@Nature nvarchar(100),
		@BtnCommand nvarchar(10)
AS
BEGIN
	IF @BtnCommand = 'Create'
		BEGIN
			INSERT INTO FTP_Server_Distributor(Code, Distributor, Country, Currency, COY_ABBR, Region, TS_Rep, MKT_Rep, Nature)
			VALUES(@Code, @Distributor, @Country, @Currency, @COY_ABBR, @Region, @TS_Rep, @MKT_Rep, @Nature)
		END
	ELSE IF @BtnCommand = 'Update'
		BEGIN 
			UPDATE FTP_Server_Distributor
			SET Code = @Code
			  , Distributor = @Distributor
			  , Country = @Country
			  , Currency = @Currency
			  , COY_ABBR = @COY_ABBR
			  , Region = @Region
			  , TS_Rep = @TS_Rep
			  , MKT_Rep = @MKT_Rep
			  , Nature = @Nature
			WHERE Code = @Code
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_FTP_Folder_Access_List]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_FTP_Folder_Access_List]
		@Access_List_UID nvarchar(30), 
		@User_ID nvarchar(10),
		@Code nvarchar(5),
		@Path nvarchar(100),
		@Folder nvarchar(50)
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM FTP_Server_Distributor_Access_List WHERE Access_List_UID = @Access_List_UID AND Folder = @Folder)
	BEGIN 
		INSERT INTO FTP_Server_Distributor_Access_List(Access_List_UID, User_ID, Code, Path, Folder)
		VALUES(@Access_List_UID, @User_ID, @Code, @Path, @Folder)
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_FTP_Users]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_FTP_Users]
		@User_Group nvarchar(30), 
		@Contact_Person nvarchar(50),
		@Email nvarchar(100),
		@User_ID nvarchar(10),
		@User_Password nvarchar(10),
		@Status nvarchar(5),
		@Code nvarchar(4)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM FTP_Server_Distributor_Account WHERE User_ID = @User_ID AND Code = @Code)
		BEGIN
			INSERT INTO FTP_Server_Distributor_Account(User_Group, Contact_Person, Email, User_ID, User_Password, Is_Active, Last_Update, Code, Access_List_UID, Remarks)
			VALUES(@User_Group, @Contact_Person, @Email, @User_ID, @User_Password, @Status, GETDATE(), @Code, @Code + '_' + @User_ID, NULL)
		END
	ELSE
		BEGIN
			UPDATE FTP_Server_Distributor_Account
			SET User_Group = @User_Group, Contact_Person = @Contact_Person, Email = @Email, User_Password = @User_Password, Is_Active = @Status, Last_Update = GETDATE()
			WHERE User_ID = @User_ID AND Code = @Code
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_LMS_Hardkey_Licence]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_LMS_Hardkey_Licence]
	@PO_No nvarchar(50),
	@PO_Date date,
	@Licence_No nvarchar(30),
	@PLU_Code nvarchar(10),
	@Start_Date date,
	@End_Date date,
	@Prepared_By nvarchar(50),
	@Sales_Representative_ID nvarchar(10),
	@Customer_ID nvarchar(20)

AS
BEGIN
	SET @Start_Date = CASE WHEN @Start_Date = '' THEN DATEADD(DAY, 1, EOMONTH(GETDATE())) ELSE @Start_Date END
	SET @End_Date = CASE WHEN @End_Date = '' THEN '2099-12-31' ELSE @End_Date END

	-- Insert record to LMS_Hardkey_Licence table
	IF NOT EXISTS(SELECT * FROM LMS_Hardkey_Licence WHERE Licence_No = @Licence_No)
	BEGIN
		INSERT INTO LMS_Hardkey_Licence(PO_No, PO_Date, SO_No, SO_Date, Invoice_No, Invoice_Date, Licence_No, PLU_Code, Created_Date, Start_Date, End_Date, Prepared_By, Customer_ID, Sales_Representative_ID)
		VALUES(@PO_No, @PO_Date, '', NULL, '', NULL, @Licence_No, @PLU_Code, GETDATE(), @Start_Date, @End_Date, @Prepared_By, @Customer_ID, @Sales_Representative_ID)
	END

	-- Insert record to DB_SO_No_By_PO table
	IF NOT EXISTS(SELECT * FROM DB_SO_No_By_PO WHERE PO_No = @PO_No)
	BEGIN
		INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date)
		SELECT @Customer_ID, @Sales_Representative_ID, @PO_No, @PO_Date
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_LMS_Licence]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_LMS_Licence]
	@Customer_ID nvarchar(20),
	@PO_No nvarchar(50),
	@PO_Date nvarchar(20),
	@Application_Type nvarchar(30),
	@Sales_Representative_ID nvarchar(10),
	@Chargeable bit,
	@OS_Type nvarchar(30),
	@Email nvarchar(100),
	@Remarks nvarchar(100),
	@AI_Account_No nvarchar(5)

AS
DECLARE @_PO_Date nvarchar(20)
DECLARE @_Invoice_No nvarchar(30)
DECLARE @_Invoice_Date nvarchar(20)

BEGIN
	SET @_PO_Date = CASE WHEN @PO_No = 'NA' THEN NULL ELSE @PO_Date END
	SET @_Invoice_No = CASE WHEN @PO_No = 'NA' THEN 'NA' ELSE '' END
	SET @_Invoice_Date = NULL

	-- Insert record to LMS_Licence table
	IF EXISTS(SELECT * FROM LMS_Licence_staging WHERE Customer_ID = @Customer_ID)
	BEGIN
		INSERT INTO LMS_Licence(Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Application_Type, OS_Type, Created_Date, Licensee_Email, Chargeable, Remarks, Customer_ID, Sales_Representative_ID, Is_Cancelled)
		SELECT TRIM(CAST(Licence_Code AS nvarchar(50))), CAST(@PO_No AS nvarchar(50)), @_PO_Date, @_Invoice_No, @_Invoice_Date, Application_Type, OS_Type, GETDATE(), @Email, @Chargeable, Remarks, @Customer_ID, @Sales_Representative_ID, 0 FROM LMS_Licence_Staging WHERE Customer_ID = @Customer_ID

		-- Insert record to DB_SO_No_By_PO table
		IF NOT EXISTS(SELECT * FROM DB_SO_No_By_PO WHERE PO_No = @PO_No)
		BEGIN
			INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date)
			SELECT @Customer_ID, @Sales_Representative_ID, @PO_No, @PO_Date
		END

		-- Update AI Gateway Key into CZL Account Table
		IF @AI_Account_No > 0
		BEGIN
			UPDATE CZL_Account 
			SET AI_Gateway_Key = (SELECT TOP 1 Licence_Code FROM LMS_Licence_staging WHERE Customer_ID = @Customer_ID)
			WHERE Client_ID = @AI_Account_No 
		END
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_LMS_Module_Licence]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_LMS_Module_Licence]
	@PO_No nvarchar(50),
	@PO_Date nvarchar(20),
	@Chargeable nvarchar(5),
	@Remarks nvarchar(100), 
	@Customer_ID nvarchar(20),
	@Sales_Representative_ID nvarchar(10)

AS
DECLARE @Module_Type nvarchar(20)
DECLARE @Quantity nvarchar(5)

DECLARE @_PO_Date nvarchar(20)
DECLARE @_Invoice_No nvarchar(30)
DECLARE @_Invoice_Date nvarchar(20)

DECLARE @UID nvarchar(20)
SET @UID = 'UID_' +  FORMAT(GETDATE(), 'yyyyMMddHHmmss')

-- 01. Insert to create an order record
BEGIN
	SET @_PO_Date = CASE WHEN @PO_No = 'NA' THEN NULL ELSE @PO_Date END
	SET @_Invoice_No = CASE WHEN @PO_No = 'NA' THEN 'NA' ELSE '' END
	SET @_Invoice_Date = NULL

	-- Insert record only when same PO No is not exist for this customer
	IF NOT EXISTS(SELECT * FROM LMS_Module_Licence_Order WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No)
		BEGIN
			INSERT INTO LMS_Module_Licence_Order(PO_No, PO_Date, Invoice_No, Invoice_Date, Created_Date, Chargeable, Remarks, Customer_ID, Sales_Representative_ID, UID, Is_Cancelled)
			VALUES(@PO_No, @_PO_Date, @_Invoice_No, @_Invoice_Date, GETDATE(), @Chargeable, @Remarks, @Customer_ID, @Sales_Representative_ID, @UID, 0)
		END
	ELSE
		IF @PO_No = 'NA'
			BEGIN
				INSERT INTO LMS_Module_Licence_Order(PO_No, PO_Date, Invoice_No, Invoice_Date, Created_Date, Chargeable, Remarks, Customer_ID, Sales_Representative_ID, UID, Is_Cancelled)
				VALUES(@PO_No, @_PO_Date, @_Invoice_No, @_Invoice_Date, GETDATE(), @Chargeable, @Remarks, @Customer_ID, @Sales_Representative_ID, @UID, 0)
			END
		ELSE
			BEGIN
				-- Delete the UID record from the LMS_Module_Licence_Order_Item table first.
				DELETE FROM LMS_Module_Licence_Order_Item WHERE UID = (SELECT UID FROM LMS_Module_Licence_Order WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No)

				-- Update the existing LMS_Module_Licence_Order table with new UID
				UPDATE LMS_Module_Licence_Order
				SET UID = @UID
				WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
			END
END



-- 02. Loop through to update module licence order quantity
DECLARE record_cursor CURSOR
FOR
	SELECT Module_Type, Quantity FROM LMS_Module_Licence_Staging
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Module_Type, @Quantity

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN
		INSERT INTO LMS_Module_Licence_Order_Item(UID, Module_Type, Quantity)
		VALUES(@UID, @Module_Type, @Quantity)
	END
	FETCH NEXT FROM record_cursor INTO @Module_Type, @Quantity
END

CLOSE record_cursor
DEALLOCATE record_cursor
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_LMS_Module_Licence_Order_Count]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_LMS_Module_Licence_Order_Count]
		@UID nvarchar(20),
		@Module_Type nvarchar(20),
		@Quantity int
AS
BEGIN
IF EXISTS(SELECT * FROM LMS_Module_Licence_Order_Item WHERE UID = @UID AND Module_Type = @Module_Type)
	BEGIN
		UPDATE LMS_Module_Licence_Order_Item 
		SET Quantity = @Quantity 
		WHERE UID = @UID AND Module_Type = @Module_Type
	END
ELSE
	BEGIN
		INSERT INTO LMS_Module_Licence_Order_Item(UID, Module_Type, Quantity)
		VALUES(@UID, @Module_Type, @Quantity)
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Banner]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Banner]
		@Banner_ID nvarchar(20),
		@Banner_Name nvarchar(50), 
		@Customer_ID nvarchar(20)
AS
BEGIN

IF NOT EXISTS(SELECT * FROM Maintenance_Banner WHERE Banner_ID = @Banner_ID AND Customer_ID = @Customer_ID)
	BEGIN
		INSERT INTO Maintenance_Banner(Banner_ID, Banner_Name, Created_Date, Last_Updated, Customer_ID)
		VALUES(dbo.Get_New_Maintenance_Banner_ID(), @Banner_Name, GETDATE(), GETDATE(), @Customer_ID)
	END
ELSE 
	BEGIN
		UPDATE Maintenance_Banner
		SET Banner_Name = @Banner_Name, 
			Last_Updated = GETDATE()
		WHERE Banner_ID = @Banner_ID AND Customer_ID = @Customer_ID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Contract]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Contract]
	@Unique_ID nvarchar(20),
	@Customer_ID nvarchar(20),
	@Store_ID nvarchar(20),
	@Start_Date date,
	@End_Date date,
	@Currency nvarchar(5),
	@Amount money,
	@Reference_No nvarchar(30),
	@Status_Code nvarchar(3)

AS
BEGIN
DECLARE @New_Maintenance_Contract_Unique_ID nvarchar(20)
SET @New_Maintenance_Contract_Unique_ID = dbo.Get_New_Maintenance_Contract_Unique_ID()

IF NOT EXISTS(SELECT * FROM Maintenance_Contract WHERE Unique_ID = @Unique_ID)
	BEGIN
		INSERT INTO Maintenance_Contract(Unique_ID, Customer_ID, Store_ID, Start_Date, End_Date, Currency, Amount, Reference_No, Status_Code)
		VALUES(@New_Maintenance_Contract_Unique_ID, @Customer_ID, @Store_ID, @Start_Date, @End_Date, @Currency, @Amount, @Reference_No, @Status_Code)
	END
ELSE 
	BEGIN
		UPDATE Maintenance_Contract
		SET Store_ID = @Store_ID, 
			Start_Date = @Start_Date,
			End_Date = @End_Date,
			Currency = @Currency,
			Amount = @Amount,
			Reference_No = @Reference_No,
			Status_Code = @Status_Code
		WHERE Unique_ID = @Unique_ID
	END
END



GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Contract_Status_Log]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Contract_Status_Log]
        @ID int,
		@Log nvarchar(max),
		@Unique_ID nvarchar(20),
		@Log_Type nvarchar(3),
		@By_Who nvarchar(50)
AS
BEGIN
IF NOT EXISTS(SELECT * FROM Maintenance_Contract_Status_Log WHERE ID = @ID)
	BEGIN
		INSERT INTO Maintenance_Contract_Status_Log(Created_Date, Log_Description, Unique_ID, Log_Type, Last_Update, By_Who)
		VALUES(GETDATE(), @Log, @Unique_ID, @Log_Type, GETDATE(), @By_Who)
	END
ELSE
	BEGIN
		UPDATE Maintenance_Contract_Status_Log
		SET Log_Description = @Log,
		    Log_Type = @Log_Type,
			Last_Update = GETDATE(),
			By_Who = @By_Who
		WHERE ID = @ID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Customer]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Customer]
        @Customer_ID nvarchar(20),
		@Name nvarchar(100),
		@Address nvarchar(255),
		@Services_Group nvarchar(1),
		@Contact_Person nvarchar(50),
		@Phone nvarchar(50),
		@Email nvarchar(100),
		@Status bit,
		@BtnCommand nvarchar(10)
AS
DECLARE @NextCustomer_ID nvarchar(20)

BEGIN
	IF @BtnCommand = 'Create'
		BEGIN
			SET @NextCustomer_ID = dbo.Get_New_Maintenance_Customer_ID()
			INSERT INTO Maintenance_Customer(Customer_ID, Name, Address, Created_Date, Is_Active, Last_Updated, Services_Group, Contact_Person, Email, Phone)
			VALUES(@NextCustomer_ID, @Name, @Address, GETDATE(), 1, GETDATE(), @Services_Group, @Contact_Person, @Email, @Phone)
		END
	ELSE IF @BtnCommand = 'Update'
		BEGIN 
			UPDATE Maintenance_Customer
			SET Name = @Name
			  , Address = @Address
			  , Last_Updated = GETDATE()
			  , Contact_Person = @Contact_Person
			  , Email = @Email
			  , Phone = @Phone
			  , Is_Active = @Status
			WHERE Customer_ID = @Customer_ID
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_ESL_Tags_Deployment]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_ESL_Tags_Deployment]
	@Unique_ID nvarchar(50),
	@Customer_ID nvarchar(20),
	@Store_ID nvarchar(20),
	@Installation_Date datetime

AS
DECLARE @Tags_Group nvarchar(20)
DECLARE @Tags_Type nvarchar(10)
DECLARE @Quantity nvarchar(5)

DECLARE @UID nvarchar(20)
SET @UID = 'UID_' +  FORMAT(GETDATE(), 'yyyyMMddHHmmss')

BEGIN
	-- Create New
	IF NOT EXISTS(SELECT * FROM Maintenance_ESL_Tags WHERE Unique_ID = @Unique_ID)
		BEGIN
			BEGIN TRANSACTION;
				INSERT INTO Maintenance_ESL_Tags(Unique_ID, Customer_ID, Store_ID, Installation_Date)
				VALUES(@UID, @Customer_ID, @Store_ID, @Installation_Date)

				DECLARE record_cursor CURSOR
				FOR
					SELECT Tags_Group, Tags_Type, Quantity FROM Maintenance_ESL_Tags_Deployment_Staging
				OPEN record_cursor
				FETCH NEXT FROM record_cursor INTO @Tags_Group, @Tags_Type, @Quantity

				WHILE @@FETCH_STATUS = 0
				BEGIN
					BEGIN
						INSERT INTO Maintenance_ESL_Tags_Deployment(Unique_ID, Tags_Group, Tags_Type, Quantity)
						VALUES(@UID, @Tags_Group, @Tags_Type, @Quantity)
					END
					FETCH NEXT FROM record_cursor INTO @Tags_Group, @Tags_Type, @Quantity
				END

				CLOSE record_cursor
				DEALLOCATE record_cursor
			COMMIT;
		END
	ELSE
		-- Edit Existing
		BEGIN
			BEGIN TRANSACTION;
				-- 01. Update the Maintenance_ESL_Tags
				UPDATE Maintenance_ESL_Tags
				SET Installation_Date = @Installation_Date
				  , Store_ID = @Store_ID
				WHERE Unique_ID = @Unique_ID

				-- 02. Loop to add and edit record in Maintenance_ESL_Tags_Deployment based on Maintenance_ESL_Tags_Deployment_Staging table
				DECLARE record_cursor CURSOR
				FOR
					SELECT Tags_Group, Tags_Type, Quantity FROM Maintenance_ESL_Tags_Deployment_Staging
				OPEN record_cursor
				FETCH NEXT FROM record_cursor INTO @Tags_Group, @Tags_Type, @Quantity

				WHILE @@FETCH_STATUS = 0
				BEGIN
					IF NOT EXISTS(SELECT * FROM Maintenance_ESL_Tags_Deployment WHERE Unique_ID = @Unique_ID AND Tags_Group = @Tags_Group AND Tags_Type = @Tags_Type)
						BEGIN
							INSERT INTO Maintenance_ESL_Tags_Deployment(Unique_ID, Tags_Group, Tags_Type, Quantity)
							SELECT @Unique_ID, @Tags_Group, @Tags_Type, @Quantity
						END
					ELSE
						BEGIN
							UPDATE Maintenance_ESL_Tags_Deployment
							SET Tags_Group = @Tags_Group
							  , Tags_Type = @Tags_Type
							  , Quantity = @Quantity
							WHERE Unique_ID = @Unique_ID AND Tags_Group = @Tags_Group AND Tags_Type = @Tags_Type
						END
					FETCH NEXT FROM record_cursor INTO @Tags_Group, @Tags_Type, @Quantity
				END

				CLOSE record_cursor
				DEALLOCATE record_cursor

				-- 03. Delete record in Maintenance_ESL_Tags_Deployment which are not in Maintenance_ESL_Tags_Deployment_Staging table
				DELETE A
				FROM Maintenance_ESL_Tags_Deployment A
				LEFT JOIN Maintenance_ESL_Tags_Deployment_Staging B ON B.Tags_Group = A.Tags_Group AND B.Tags_Type = A.Tags_Type
				WHERE B.Tags_Group IS NULL AND B.Tags_Type IS NULL
				AND A.Unique_ID = @Unique_ID
			COMMIT;
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_ESL_Tags_Type]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_ESL_Tags_Type]
		@ID int,
		@Tags_Group nvarchar(20),
		@Tags_Type nvarchar(10)
AS
--BEGIN

--IF NOT EXISTS(SELECT * FROM Maintenance_ESL_Tags_Type WHERE ID = @ID)
--	BEGIN
--		INSERT INTO Maintenance_ESL_Tags_Type(Tags_Group, Tags_Type)
--		VALUES(@Tags_Group, @Tags_Type)
--	END
--ELSE 
--	BEGIN
--		UPDATE Maintenance_ESL_Tags_Deployment
--		SET Tags_Group = @Tags_Group, Tags_Type = @Tags_Type
--		WHERE Tags_Group = (SELECT Tags_Group FROM Maintenance_ESL_Tags_Type WHERE ID = @ID) AND Tags_Type = (SELECT Tags_Type FROM Maintenance_ESL_Tags_Type WHERE ID = @ID)

--		UPDATE Maintenance_ESL_Tags_Type
--		SET Tags_Group = @Tags_Group, 
--			Tags_Type = @Tags_Type
--		WHERE ID = @ID
--	END
--END

BEGIN TRY
    BEGIN TRANSACTION

    IF NOT EXISTS(SELECT * FROM Maintenance_ESL_Tags_Type WHERE ID = @ID)
        BEGIN
            INSERT INTO Maintenance_ESL_Tags_Type(Tags_Group, Tags_Type)
            VALUES(@Tags_Group, @Tags_Type)
        END
    ELSE 
        BEGIN
            -- Capture the old values before they are updated
            DECLARE @Old_Tags_Group VARCHAR(255), @Old_Tags_Type VARCHAR(255)

            SELECT @Old_Tags_Group = Tags_Group, @Old_Tags_Type = Tags_Type
            FROM Maintenance_ESL_Tags_Type 
            WHERE ID = @ID

            -- Update Deployment table
            UPDATE Maintenance_ESL_Tags_Deployment
            SET Tags_Group = @Tags_Group, Tags_Type = @Tags_Type
            WHERE Tags_Group = @Old_Tags_Group AND Tags_Type = @Old_Tags_Type

            -- Update Type table
            UPDATE Maintenance_ESL_Tags_Type
            SET Tags_Group = @Tags_Group, 
                Tags_Type = @Tags_Type
            WHERE ID = @ID
        END

    -- If we reach here, it means everything went fine, so commit the transaction
    COMMIT TRANSACTION
END TRY
BEGIN CATCH
    -- If there is any error, roll back the entire transaction
    ROLLBACK TRANSACTION
    -- You can log the error or re-throw it here depending on your application needs
    -- THROW; -- Uncomment this if you want to throw the error to the application
END CATCH

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Product]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Product]
        @Unique_ID nvarchar(20),
		@Serial_No nvarchar(20),
		@Install_Date date,
		@Warranty_Start_Date date,
		@Warranty_Cover_Period int,
		@Value_Currency nvarchar(3),
		@Product_Value money,
		@Product_Code nvarchar(20),
		@Store_ID nvarchar(20),
		@Customer_ID nvarchar(20)
AS
BEGIN

IF NOT EXISTS(SELECT * FROM Maintenance_Product WHERE Unique_ID = @Unique_ID)
	BEGIN
		INSERT INTO Maintenance_Product(Unique_ID, Serial_No, Created_Date, Installation_Date, Usage_Start_Date, Warranty_Expiration, Value_Currency, Product_Value, Last_Updated, Product_Code, Store_ID, Customer_ID)
		VALUES(dbo.Get_New_Maintenance_Product_Unique_ID(), @Serial_No, GETDATE(), @Install_Date, @Warranty_Start_Date, DATEADD(DAY, -1, DATEADD(MONTH, @Warranty_Cover_Period, @Warranty_Start_Date)), @Value_Currency, @Product_Value, GETDATE(), @Product_Code, @Store_ID, @Customer_ID)
	END
ELSE 
	BEGIN
		UPDATE Maintenance_Product
		SET Serial_No = @Serial_No, 
		    Product_Code = @Product_Code,
			Store_ID = @Store_ID,
			Installation_Date = @Install_Date,
			Usage_Start_Date = @Warranty_Start_Date,
			Warranty_Expiration = DATEADD(DAY, -1, DATEADD(MONTH, @Warranty_Cover_Period, @Warranty_Start_Date)),
			Value_Currency = @Value_Currency,
			Product_Value = @Product_Value,
			Last_Updated = GETDATE()
		WHERE Unique_ID = @Unique_ID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Product_Type]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Product_Type]
		@UID nvarchar(20),
		@Code nvarchar(20),
		@Product_Name nvarchar(100),
		@Category nvarchar(20),
		@Services_Group nvarchar(1)
AS
BEGIN

SET @Category = CASE WHEN @Category = '-1' THEN '' ELSE @Category END

IF NOT EXISTS(SELECT * FROM Maintenance_Product_Type WHERE UID = @UID)
	BEGIN
		INSERT INTO Maintenance_Product_Type(UID, Code, Product_Name, Category, Services_Group)
		VALUES(dbo.Get_New_Maintenance_Product_Type_Unique_ID(), @Code, @Product_Name, @Category, @Services_Group)
	END
ELSE 
	BEGIN
		UPDATE Maintenance_Product_Type
		SET Code = @Code, 
			Product_Name = @Product_Name,
			Category = @Category
		WHERE UID = @UID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Maintenance_Store]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Maintenance_Store]
		@Store_ID nvarchar(20),
		@Store_Name nvarchar(50),
		@Status bit,
		@Banner_ID nvarchar(20), 
		@Customer_ID nvarchar(20)
AS
BEGIN

IF NOT EXISTS(SELECT * FROM Maintenance_Store WHERE Store_ID = @Store_ID AND Banner_ID = @Banner_ID AND Customer_ID = @Customer_ID)
	BEGIN
		INSERT INTO Maintenance_Store(Store_ID, Store_Name, Created_Date, Last_Updated, Is_Active, Banner_ID, Customer_ID)
		VALUES(dbo.Get_New_Maintenance_Store_ID(), @Store_Name, GETDATE(), GETDATE(), 1, @Banner_ID, @Customer_ID)
	END
ELSE 
	BEGIN
		UPDATE Maintenance_Store
		SET Store_Name = @Store_Name, 
			Last_Updated = GETDATE(),
			Is_Active = @Status,
			Banner_ID = @Banner_ID
		WHERE Store_ID = @Store_ID AND Banner_ID = @Banner_ID AND Customer_ID = @Customer_ID
	END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Notes]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Notes]
		@Customer_ID nvarchar(20),
		@Notes nvarchar(max),
		@Notes_For nvarchar(30),
		@BtnCommand nvarchar(10)
AS
BEGIN
	IF @BtnCommand = 'Create'
		BEGIN
			INSERT INTO DB_Account_Notes(Customer_ID, Notes, Added_Date, Notes_For)
			VALUES(@Customer_ID, @Notes, GETDATE(), @Notes_For)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Recovered_Invoice_Bill_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Recovered_Invoice_Bill_Items]
	@Invoice_No nvarchar(30),
	@Invoice_Date date,
	@Old_Item_Code nvarchar(50),
	@Item_Code nvarchar(50),
	@Currency nvarchar(5),
	@Amount money
AS
DECLARE @Customer_ID nvarchar(20)
BEGIN
	SET @Customer_ID = ( CASE WHEN EXISTS(SELECT H.Customer_ID FROM DMC_Subscription S
                                          INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = SUBSTRING(S.Store_ID, 2, 6)
                                          WHERE Ref_Invoice_No = @Invoice_No)
	                          THEN (SELECT TOP 1 H.Customer_ID FROM DMC_Subscription S
                                    INNER JOIN DMC_Headquarter H ON H.Headquarter_ID = SUBSTRING(S.Store_ID, 2, 6)
                                    WHERE Ref_Invoice_No = @Invoice_No)
							  ELSE CASE WHEN EXISTS(SELECT * FROM LMS_Licence WHERE Invoice_No = @Invoice_No)
							            THEN (SELECT TOP 1 Customer_ID FROM LMS_Licence WHERE Invoice_No = @Invoice_No)
										ELSE CASE WHEN EXISTS(SELECT * FROM LMS_Termed_Licence_Renewal WHERE Invoice_No = @Invoice_No) 
										          THEN (SELECT TOP 1 Customer_ID FROM LMS_Termed_Licence_Renewal WHERE Invoice_No = @Invoice_No)
												  ELSE (SELECT TOP 1 Customer_ID FROM LMS_Hardkey_Licence WHERE Invoice_No = @Invoice_No)
												  END
										END
							  END )

	IF EXISTS (SELECT * FROM DB_Recovered_Invoice WHERE Invoice_No = @Invoice_No AND Item_Code = @Old_Item_Code)
		UPDATE DB_Recovered_Invoice
		SET Item_Code = @Item_Code, Currency = @Currency, Amount = @Amount
		WHERE Invoice_No = @Invoice_No AND Item_Code = @Old_Item_Code
	ELSE
		IF @Item_Code != 'DMC004' AND @Item_Code != 'DMC005' AND @Item_Code != 'DMC013'
		BEGIN 
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID)
			VALUES(@Invoice_No, @Invoice_Date, @Item_Code, @Currency, @Amount, @Customer_ID)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Reminder]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Reminder]
		@Reminder nvarchar(500)
AS
BEGIN
	IF NOT EXISTS (SELECT * FROM DB_Reminder WHERE Reminder = @Reminder AND Completed_Date IS NULL)
		BEGIN
			INSERT INTO DB_Reminder(Created_Date, Reminder, Completed_Date, Is_Done)
			VALUES(GETDATE(), @Reminder, NULL, 0)
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_CRUD_Termed_Licence_Renewal]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_CRUD_Termed_Licence_Renewal] 
	@PO_No nvarchar(50),
	@Customer_ID nvarchar(20)

AS
DECLARE @TermedLicenceRenewalID nvarchar(20)
SET @TermedLicenceRenewalID = (SELECT dbo.Get_New_Termed_Licence_Renewal_ID())

BEGIN
	IF EXISTS(SELECT * FROM LMS_Termed_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No)
	BEGIN
		INSERT INTO LMS_Termed_Licence_Renewal(Renewal_UID, Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID)
		SELECT @TermedLicenceRenewalID, Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID FROM LMS_Termed_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
	END

	-- Insert record to DB_SO_No_By_PO table
	IF NOT EXISTS(SELECT * FROM DB_SO_No_By_PO WHERE PO_No = @PO_No)
	BEGIN
		INSERT INTO DB_SO_No_By_PO(Customer_ID, Sales_Representative_ID, PO_No, PO_Date)
		SELECT TOP 1 Customer_ID, Sales_Representative_ID, PO_No, PO_Date FROM LMS_Termed_Licence_Renewal WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Renewal_UID = @TermedLicenceRenewalID
	END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_D_Sales_Item_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_D_Sales_Item_Summary]
AS
BEGIN
DECLARE @sqlcols AS nvarchar(MAX)
DECLARE @pvtcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @sqlcols = STUFF(( SELECT [Year] FROM ( SELECT DISTINCT ', ISNULL(' + QUOTENAME([YEAR]) + ', 0) AS ' + QUOTENAME([YEAR]) AS [Year]
						                    FROM DB_List_Of_Year
											WHERE [Year] > YEAR(GETDATE()) - 5
						                  ) TBL 
					   ORDER BY [YEAR] DESC
                    FOR XML PATH(''), TYPE
                   ).value('.', 'NVARCHAR(MAX)') 
                   ,1,1,'')

SET @pvtcols = STUFF(( SELECT [Year] FROM ( SELECT DISTINCT ', ' + QUOTENAME([YEAR]) AS [Year]
						                    FROM DB_List_Of_Year
											WHERE [Year] > YEAR(GETDATE()) - 5
						                  ) TBL 
					   ORDER BY [YEAR] DESC
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = '  SELECT [Item Code], [Type], [Description],' + @sqlcols +
		     '  FROM (
						SELECT Item_Code AS [Item Code]
							 , (SELECT Value_4 FROM DB_Lookup WHERE Value_1 = Item_Code) AS [Type]
							 , Description
							 , YEAR(Invoice_Date) AS [Year]
							 , SUM(CASE WHEN Currency = ''SGD'' THEN Amount ELSE dbo.Currency_Conversion(Invoice_Date, Currency, Amount) END) AS Amount_In_SGD
						FROM I_DB_Recovered_Invoice I
						WHERE YEAR(Invoice_Date) > YEAR(GETDATE()) - 5
						GROUP BY Item_Code, Description, YEAR(Invoice_Date)
				     ) SRC
		        PIVOT 
		        (
			        SUM([Amount_In_SGD])
			        FOR [Year] IN (' + @pvtcols + ')
		        ) PVT ORDER BY CASE PVT.[Type] WHEN ''DMC'' THEN 1 WHEN ''App Licence'' THEN 2 WHEN ''SM Module Licence'' THEN 3 ELSE 4 END '

EXECUTE(@query)

END



GO
/****** Object:  StoredProcedure [dbo].[SP_ESL_Tags_Deployment]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_ESL_Tags_Deployment] 
	@Customer_ID nvarchar(20), @Store_Name nvarchar(50)

AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT * 
                    FROM (  SELECT DISTINCT + ', SUM(ISNULL(' + QUOTENAME([Tags Category]) + ', 0)) AS ' + QUOTENAME([Tags Category]) AS QueryColName
                            FROM R_Maintenance_ESL_Tags_Deployment
							WHERE [Customer ID] = @Customer_ID
					     ) TBL 
					ORDER BY QueryColName
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT * 
                    FROM (
					        SELECT DISTINCT + ', ' + QUOTENAME([Tags Category]) AS QueryColName
                            FROM R_Maintenance_ESL_Tags_Deployment
					      ) TBL 
						  ORDER BY QueryColName
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT R_Maintenance_ESL_Tags.[Unique ID]
                    , [Customer ID]
					, [Customer Name]
					, FORMAT([Installation Date], ''dd MMM yy'') AS [Installation Date]
					, [Store ID]
					, [Store Name]
					, [Services Group]
					, [Editable]
	                , ' + @cols +
             '  FROM R_Maintenance_ESL_Tags
                INNER JOIN ( SELECT *
			                FROM ( SELECT [Unique ID], [Tags Group], [Tags Type], [Tags Category], [Quantity] 
							FROM R_Maintenance_ESL_Tags_Deployment 
							) AS SourceTable
							PIVOT
							(
							SUM([Quantity])
							FOR [Tags Category] IN (' + @PVcols + ')
							) AS PivotTable
				) I ON I.[Unique ID] = R_Maintenance_ESL_Tags.[Unique ID]
			    WHERE [Customer ID] = ''' + @Customer_ID + ''' AND [Store Name] LIKE ''%' + @Store_Name +'%''
			    GROUP BY R_Maintenance_ESL_Tags.[Unique ID], [Customer ID], [Customer Name], [Store ID], [Store Name], [Installation Date], [Services Group], [Editable]
				ORDER BY R_Maintenance_ESL_Tags.[Unique ID] DESC '

EXECUTE(@query)


END
GO
/****** Object:  StoredProcedure [dbo].[SP_ESL_Tags_Deployment_Overview]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_ESL_Tags_Deployment_Overview] 

AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT * 
                    FROM (  SELECT DISTINCT + ', SUM(ISNULL(' + QUOTENAME([Tags Category]) + ', 0)) AS ' + QUOTENAME([Tags Category]) AS QueryColName
                            FROM R_Maintenance_ESL_Tags_Deployment
					     ) TBL 
					ORDER BY QueryColName
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT * 
                    FROM (
					        SELECT DISTINCT + ', ' + QUOTENAME([Tags Category]) AS QueryColName
                            FROM R_Maintenance_ESL_Tags_Deployment
					      ) TBL 
						  ORDER BY QueryColName
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT [Customer Name]
					, CASE WHEN GROUPING([Store Name]) = 1 THEN ''TOTAL'' ELSE [Store Name] END AS [Store Name]
	                , ' + @cols +
             '  FROM R_Maintenance_ESL_Tags
                INNER JOIN ( SELECT *
			                FROM ( SELECT [Unique ID], [Tags Group], [Tags Type], [Tags Category], [Quantity] 
							FROM R_Maintenance_ESL_Tags_Deployment 
							) AS SourceTable
							PIVOT
							(
							SUM([Quantity])
							FOR [Tags Category] IN (' + @PVcols + ')
							) AS PivotTable
				) I ON I.[Unique ID] = R_Maintenance_ESL_Tags.[Unique ID]
			    GROUP BY GROUPING SETS (([Customer Name], [Store Name]), ())
			    ORDER BY GROUPING([Customer Name]),[Customer Name] '

EXECUTE(@query)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Generate_Server_Consumption_Statistics]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE   PROCEDURE [dbo].[SP_Generate_Server_Consumption_Statistics]
    @tablename nvarchar(128),
    @colname1 nvarchar(128),
    @colname2 nvarchar(128)
AS
BEGIN
DECLARE @SqlQuery nvarchar(max)

SET @tablename = REPLACE(@tablename, '_Growth', '_Moving_Average')

SET @SqlQuery = ' DECLARE @UsedMedianValue decimal(10, 2) ' +
                ' DECLARE @DBMedianValue decimal(10, 2) ' +
				' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname1 + ''', @UsedMedianValue OUTPUT; ' +
				' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname2 + ''', @DBMedianValue OUTPUT; ' +
                ' SELECT ''Used Growth'' AS [Statistics], (SELECT TOP 1 [Space Growth Moving Average] FROM ' + @tablename + ' ORDER BY [Start Date] DESC) AS [Mean], @UsedMedianValue AS [Median] FROM ' + @tablename +
                ' UNION ' +
                ' SELECT ''DB Growth'' AS [Statistics], (SELECT TOP 1 [DB Growth Moving Average] FROM ' + @tablename + ' ORDER BY [Start Date] DESC) AS [Mean], @DBMedianValue AS Median FROM ' + @tablename +
				' ORDER BY [Statistics] DESC '

--SET @SqlQuery = 'DECLARE @UsedMedianValue decimal(10, 2) ' +
--                'DECLARE @DBMedianValue decimal(10, 2) ' +
--				' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname1 + ''', @UsedMedianValue OUTPUT; EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname2 + ''', @DBMedianValue OUTPUT; ' +
--                ' SELECT ''Used Growth'' AS [Statistics], CAST(AVG([' + @colname1 + ']) AS decimal(10, 2)) AS [Mean], @UsedMedianValue AS [Median], CAST(STDEVP([' + @colname1 + ']) AS decimal(10, 2)) AS [Standard Deviation] FROM ' + @tablename +
--                ' UNION ' +
--                ' SELECT ''DB Growth'' AS [Statistics], CAST(AVG([' + @colname2 + ']) AS decimal(10, 2)) AS [Mean], @DBMedianValue AS Median, CAST(STDEVP([' + @colname2 + ']) AS decimal(10, 2)) AS [Standard Deviation] FROM ' + @tablename +
--				' ORDER BY [Statistics] DESC '

--SET @SqlQuery = ' DECLARE @UsedMedianValue decimal(10, 2) ' +
--                --' DECLARE @AvailableMedianValue decimal(10, 2)  ' +
--                ' DECLARE @DBMedianValue decimal(10, 2) ' +
--				' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname1 + ''', @UsedMedianValue OUTPUT; ' +
--				--' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname2 + ''', @AvailableMedianValue OUTPUT; ' +
--				' EXEC dbo.SP_Get_Median ''' + @tablename + ''', ''' + @colname2 + ''', @DBMedianValue OUTPUT; ' +
--                ' SELECT ''Used Growth'' AS [Statistics], CAST(AVG([' + @colname1 + ']) AS decimal(10, 2)) AS [Mean], @UsedMedianValue AS [Median] FROM ' + @tablename +
--                ' UNION ' +
--                --' SELECT ''Avail Diff'' AS [Statistics], CAST(AVG([' + @colname2 + ']) AS decimal(10, 2)) AS [Mean], @AvailableMedianValue AS [Median] FROM ' + @tablename +
--                --' UNION ' +
--                ' SELECT ''DB Growth'' AS [Statistics], CAST(AVG([' + @colname2 + ']) AS decimal(10, 2)) AS [Mean], @DBMedianValue AS Median FROM ' + @tablename +
--				' ORDER BY [Statistics] DESC '

EXECUTE(@SqlQuery)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Get_Median]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Get_Median]
    @tablename nvarchar(128),
    @colname nvarchar(128),
    @Median decimal(10, 2) OUTPUT
AS
BEGIN
    DECLARE @SqlQuery NVARCHAR(MAX)

    SET @SqlQuery = N''

    IF @colname = 'Used Growth'
		BEGIN
			SET @SqlQuery = N'
				SELECT @Median = (
					(SELECT TOP 1 [' + @colname + ']
					 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + ']) AS A
					 ORDER BY [' + @colname + '] DESC)
					+
					(SELECT TOP 1 [' + @colname + ']
					 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + '] DESC) AS B
					 ORDER BY [' + @colname + '] ASC)
				) / 2'
		END
	--ELSE IF @colname = 'Avail Diff'
	--	BEGIN
	--		SET @SqlQuery = N'
	--			SELECT @Median = (
	--				(SELECT TOP 1 [' + @colname + ']
	--				 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + ']) AS A
	--				 ORDER BY [' + @colname + '] DESC)
	--				+
	--				(SELECT TOP 1 [' + @colname + ']
	--				 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + '] DESC) AS B
	--				 ORDER BY [' + @colname + '] ASC)
	--			) / 2'			
	--	END
    ELSE IF @colname = 'DB Growth'
		BEGIN
			SET @SqlQuery = N'
				SELECT @Median = (
					(SELECT TOP 1 [' + @colname + ']
					 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + ']) AS A
					 ORDER BY [' + @colname + '] DESC)
					+
					(SELECT TOP 1 [' + @colname + ']
					 FROM (SELECT TOP 50 PERCENT [' + @colname + '] FROM ' + QUOTENAME(@tablename) + ' ORDER BY [' + @colname + '] DESC) AS B
					 ORDER BY [' + @colname + '] ASC)
				) / 2'
		END

    EXEC sp_executesql @SqlQuery, N'@Median decimal(10, 2) OUTPUT', @Median OUTPUT

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_AI_Licence_Renewal_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_AI_Licence_Renewal_Recovered_Invoice_Items]
	@UID nvarchar(20),
	@PO_No nvarchar(50)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Item_Code nvarchar(50)
DECLARE @Chargeable int
DECLARE @Customer_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR
	SELECT DISTINCT Invoice_No, Invoice_Date, CAST(Chargeable As int), Customer_ID 
	FROM LMS_AI_Licence_Renewal 
	WHERE Renewal_UID = @UID
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Chargeable, @Customer_ID

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN	
		SET @Item_Code = (SELECT TOP 1 Value_1 FROM DB_Lookup WHERE Value_3 = 'AI' ORDER BY ID DESC)
		IF @Chargeable = 1
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID, PO_No) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, '', 0, @Customer_ID, @PO_No)
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Chargeable, @Customer_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_AI_Licence_Renewal_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_AI_Licence_Renewal_Staging]
	@Licence_Code nvarchar(50),
	@PO_No nvarchar(20),
	@PO_Date nvarchar(10),
	@Chargeable bit,
	@Currency nvarchar(5),
	@Fee money,
	@Remarks nvarchar(100),
	@Customer_ID nvarchar(20),
	@Sales_Representative_ID nvarchar(10)

AS
DECLARE @_PO_Date nvarchar(20)
DECLARE @_Invoice_No nvarchar(30)
DECLARE @_Invoice_Date nvarchar(20)

SET @_PO_Date = CASE WHEN @PO_No = 'NA' THEN NULL ELSE @PO_Date END
SET @_Invoice_No = CASE WHEN @PO_No = 'NA' THEN 'NA' ELSE '' END
SET @_Invoice_Date = NULL

BEGIN
	IF NOT EXISTS(SELECT * FROM LMS_AI_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Licence_Code = @Licence_Code)
		BEGIN
			INSERT INTO LMS_AI_Licence_Renewal_Staging(Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID)
			VALUES(@Licence_Code, @PO_No, @_PO_Date, @_Invoice_No, @_Invoice_Date, GETDATE(), @Chargeable, @Currency, @Fee, @Remarks, @Customer_ID, @Sales_Representative_ID)
		END
	ELSE
		BEGIN
			UPDATE LMS_AI_Licence_Renewal_Staging
			SET Licence_Code = @Licence_Code
			  , PO_No = @PO_No
			  , PO_Date = @_PO_Date
			  , Invoice_No = ''
			  , Invoice_Date = NULL
			  , Chargeable = @Chargeable
			  , Currency = @Currency
			  , Fee = @Fee
			  , Remarks = @Remarks
			WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Licence_Code = @Licence_Code
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_App_Product_Licence_Order_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_App_Product_Licence_Order_Recovered_Invoice_Items]
	@Customer_ID nvarchar(50),
	@PO_No nvarchar(50)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Application_Type nvarchar(100)
DECLARE @Item_Code nvarchar(50)
DECLARE @Chargeable int

DECLARE record_cursor CURSOR
FOR
	SELECT DISTINCT Invoice_No, Invoice_Date, Application_Type, CAST(Chargeable As int) 
	FROM LMS_Licence 
	WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Application_Type, @Chargeable 

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN	
		SET @Item_Code = (SELECT TOP 1 Value_1 FROM DB_Lookup WHERE Value_3 = @Application_Type ORDER BY ID DESC)
		IF @Chargeable = 1
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID, PO_No) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, '', 0, @Customer_ID, CAST(@PO_No AS nvarchar(50)))
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Application_Type, @Chargeable
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_CZL_Account_Setup_Charge_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_CZL_Account_Setup_Charge_Recovered_Invoice_Items]
	@CZL_Account_Unique_ID nvarchar(20),
	@PO_No nvarchar(50)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Item_Code nvarchar(50)
DECLARE @Currency nvarchar(5)
DECLARE @Amount money
DECLARE @Distributor_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR
	SELECT Invoice_No, Invoice_Date, Currency, Fee, Sales_Representative_ID 
	FROM CZL_Account_Setup_Charge 
	WHERE CZL_Account_Unique_ID = @CZL_Account_Unique_ID
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Currency, @Amount, @Distributor_ID

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN	
		SET @Item_Code = (SELECT TOP 1 Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_2 LIKE '%AI Account Setup Fee%' ORDER BY ID DESC)
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID, PO_No) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, @Currency, @Amount, @Distributor_ID, CAST(@PO_No AS nvarchar(5)))
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Currency, @Amount, @Distributor_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_CZL_Model_Update_Charge_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_CZL_Model_Update_Charge_Recovered_Invoice_Items]
	@CZL_Account_ID nvarchar(20),
	@UID nvarchar(20)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Item_Code nvarchar(50)
DECLARE @Currency nvarchar(5)
DECLARE @Fee money
DECLARE @Distributor_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR
	SELECT [Invoice No], [Invoice Date], [Currency], [Fee], [Distributor ID]
	FROM I_CZL_Account_Model_Upgrade_Charge 
	WHERE [UID] = @UID
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Currency, @Fee, @Distributor_ID

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN	
		SET @Item_Code = (SELECT TOP 1 Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Bill Items' AND Value_2 LIKE '%Model Update%' ORDER BY ID DESC)
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), ISNULL(@Item_Code, '00000'), @Currency, @Fee, @Distributor_ID)
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Currency, @Fee, @Distributor_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Hardkey_Order_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Hardkey_Order_Recovered_Invoice_Items]
	@Customer_ID nvarchar(50),
	@PO_No nvarchar(50)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Item_Code nvarchar(50)

DECLARE record_cursor CURSOR
FOR
	SELECT DISTINCT Invoice_No, Invoice_Date, PLU_Code
	FROM LMS_Hardkey_Licence
	WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Item_Code

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN
		If @Invoice_No != 'NA'
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, '', 0, @Customer_ID)
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Item_Code
END
CLOSE record_cursor
DEALLOCATE record_cursor
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Module_Licence_Order_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Module_Licence_Order_Recovered_Invoice_Items]
	@UID nvarchar(20),
	@Invoice_No nvarchar(30),
	@Invoice_Date nvarchar(30),
	@PO_No nvarchar(50)

AS
DECLARE @Item_Code nvarchar(5)
DECLARE @ModuleLicenceName nvarchar(20)
DECLARE @ModuleCount int
DECLARE @CountTempTable table (OrderCount int)
DECLARE @Customer_ID nvarchar(20)

DECLARE record_cursor CURSOR
FOR
	SELECT DISTINCT Value_1 FROM DB_Lookup 
	WHERE Value_4 = 'SM Module Licence'  AND Value_3 IN (SELECT Module_Type FROM LMS_Module_Licence_Order_Item WHERE UID = @UID)
	ORDER BY Value_1
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Item_Code

BEGIN
	-- Do the insert while module licence item code still exits 
	WHILE @@FETCH_STATUS = 0
	BEGIN			
			-- Get Module Licence Name
			SET @ModuleLicenceName = (SELECT TOP 1 Value_3 FROM DB_Lookup WHERE Value_1 = @Item_Code AND Value_4 = 'SM Module Licence')
			
			-- Assign Temp Table
			INSERT @CountTempTable 
			EXECUTE('SELECT Quantity FROM LMS_Module_Licence_Order_Item WHERE Module_Type = ''' + @ModuleLicenceName + ''' AND UID = ''' + @UID + '''')
			
			-- Get Order count
			SELECT @ModuleCount = OrderCount FROM @CountTempTable

			-- Get Customer ID of the order
			SET @Customer_ID = (SELECT TOP 1 Customer_ID FROM LMS_Module_Licence_Order WHERE UID = @UID )

			IF @ModuleCount > 0
			BEGIN
				BEGIN
					INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID, PO_No) 
					VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, '', 0, @Customer_ID, @PO_No)
				END
			END
			FETCH NEXT FROM record_cursor INTO @Item_Code
	END
	CLOSE record_cursor
	DEALLOCATE record_cursor
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Module_Licence_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Module_Licence_Staging]
	@Customer_ID nvarchar(20),
	@PO_No nvarchar(50),
	@Module_Type nvarchar(20),
	@Quantity int

AS
BEGIN
	IF EXISTS(SELECT * FROM LMS_Module_Licence_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Module_Type = @Module_Type)
		BEGIN
			UPDATE LMS_Module_Licence_Staging
			SET Quantity = @Quantity
			WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Module_Type = @Module_Type
		END
	ELSE
		BEGIN
			INSERT INTO LMS_Module_Licence_Staging(Customer_ID, PO_No, Module_Type, Quantity)
			VALUES(@Customer_ID, @PO_No, @Module_Type, @Quantity)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Subscription_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Subscription_Staging]
	@Headquarter_ID nvarchar(20),
	@Store_ID nvarchar(20),
	@Duration nvarchar(20),
	@Currency nvarchar(5),
	@Fee money,
	@Payment_Method nvarchar(20),
	@Payment_Mode nvarchar(20),
	@Subscriber_Group nvarchar(1)

AS
DECLARE @Account_No nvarchar(50)
DECLARE @Curr_Date date 
DECLARE @Synced_dmcstore_userstoreid nvarchar(10)

BEGIN
	IF EXISTS(SELECT * FROM DMC_Subscription_Staging WHERE Store_ID = @Store_ID)
		BEGIN
			UPDATE DMC_Subscription_Staging
			SET Duration = @Duration
			  , Currency = @Currency
			  , Fee = @Fee
			  , Payment_Method = @Payment_Method
			  , Payment_Mode = @Payment_Mode
			  , Subscriber_Group = @Subscriber_Group
			WHERE Store_ID = @Store_ID AND Headquarter_ID = @Headquarter_ID
		END
	ELSE
		BEGIN
			SET @Synced_dmcstore_userstoreid = (SELECT TOP 1 Synced_dmcstore_userstoreid FROM DMC_Store WHERE Store_ID = @Store_ID)

			INSERT INTO DMC_Subscription_Staging(Headquarter_ID, Store_ID, Synced_dmcstore_userstoreid, Duration, Currency, Fee, Payment_Method, Payment_Mode, Subscriber_Group)
			VALUES(@Headquarter_ID, @Store_ID, @Synced_dmcstore_userstoreid, @Duration, @Currency, @Fee, @Payment_Method, @Payment_Mode, @Subscriber_Group)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Tags_Deployment_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Tags_Deployment_Staging]
	@Customer_ID nvarchar(20),
	@Store_ID nvarchar(20),
	@Tags_Group nvarchar(20),
	@Tags_Type nvarchar(10),
	@Quantity int

AS
BEGIN
	IF EXISTS(SELECT * FROM Maintenance_ESL_Tags_Deployment_Staging WHERE Customer_ID = @Customer_ID AND Store_ID = @Store_ID AND Tags_Group = @Tags_Group AND Tags_Type = @Tags_Type)
		BEGIN
			UPDATE Maintenance_ESL_Tags_Deployment_Staging
			SET Quantity = @Quantity
			WHERE Customer_ID = @Customer_ID AND Store_ID = @Store_ID AND Tags_Group = @Tags_Group AND Tags_Type = @Tags_Type
		END
	ELSE
		BEGIN
			INSERT INTO Maintenance_ESL_Tags_Deployment_Staging(Customer_ID, Store_ID, Tags_Group, Tags_Type, Quantity)
			VALUES(@Customer_ID, @Store_ID, @Tags_Group, @Tags_Type, @Quantity)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary] @StartDate date, @EndDate date, @Country nvarchar(200), @Device_Type nvarchar(20)
AS
DECLARE @monthly as date

DECLARE record_cursor CURSOR
FOR 
	SELECT EOMONTH(MonthYearList) 
	FROM dbo.Get_MonthYearList(@StartDate, @EndDate)
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @monthly

-- Drop table if exists
DROP TABLE IF EXISTS [Headquarter_Device_Type]

-- Create the table
CREATE TABLE [dbo].[Headquarter_Device_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Device_Type] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Headquarter_Device_Type(Headquarter_ID, Device_Type)
SELECT Headquarter_ID, Device_Type FROM _HeadquarterDeviceType

-- Drop table if exists
DROP TABLE IF EXISTS [TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary]

-- Create the table
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] int NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

WHILE @@FETCH_STATUS = 0
BEGIN
		IF @Country != 'ALL'
			BEGIN
				IF @Device_Type != 'ALLOFALL'
					BEGIN
						INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
						SELECT YEAR(@monthly) AS [Year]
							 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
							 , SUM(Total_Amount_Per_Month) AS [Total Amount]
							 , SUM(Owned_Store) AS [No Of Store]
							 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
						FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@monthly)  
						WHERE Country = @Country AND Device_Type = @Device_Type
					END
				ELSE
					BEGIN
						INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
						SELECT YEAR(@monthly) AS [Year]
							 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
							 , SUM(Total_Amount_Per_Month) AS [Total Amount]
							 , SUM(Owned_Store) AS [No Of Store]
							 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
						FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@monthly)  
						WHERE Country = @Country					
					END
			END
		ELSE 
			IF @Device_Type != 'ALLOFALL'
				BEGIN
					INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
					SELECT YEAR(@monthly) AS [Year]
						 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
						 , SUM(Total_Amount_Per_Month) AS [Total Amount]
						 , SUM(Owned_Store) AS [No Of Store]
						 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
					FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@monthly)  
					WHERE Device_Type = @Device_Type				
				END
			 ELSE
				BEGIN
					INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Base_USD_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
					SELECT YEAR(@monthly) AS [Year]
						 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
						 , SUM(Total_Amount_Per_Month) AS [Total Amount]
						 , SUM(Owned_Store) AS [No Of Store]
						 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
					FROM dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@monthly)  			 
				END
	FETCH NEXT FROM record_cursor INTO @monthly
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary] @StartDate date, @EndDate date, @Country nvarchar(200), @Device_Type nvarchar(20)
AS
DECLARE @monthly as date

DECLARE record_cursor CURSOR
FOR 
	SELECT EOMONTH(MonthYearList) 
	FROM dbo.Get_MonthYearList(@StartDate, @EndDate)
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @monthly

-- Drop table if exists
DROP TABLE IF EXISTS [Headquarter_Device_Type]

-- Create the table
CREATE TABLE [dbo].[Headquarter_Device_Type](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Headquarter_ID] [nvarchar](20) NULL,
	[Device_Type] [nvarchar](20) NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

INSERT INTO Headquarter_Device_Type(Headquarter_ID, Device_Type)
SELECT Headquarter_ID, Device_Type FROM _HeadquarterDeviceType

-- Drop table if exists
DROP TABLE IF EXISTS [TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary]

-- Create the table
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] int NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

WHILE @@FETCH_STATUS = 0
BEGIN
		IF @Country != 'ALL'
			BEGIN
				IF @Device_Type != 'ALLOFALL'
					BEGIN
						INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
						SELECT YEAR(@monthly) AS [Year]
							 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
							 , SUM(Total_Amount_Per_Month) AS [Total Amount]
							 , SUM(Owned_Store) AS [No Of Store]
							 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
						FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@monthly)  
						WHERE Country = @Country AND Device_Type = @Device_Type
					END
				ELSE
					BEGIN
						INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
						SELECT YEAR(@monthly) AS [Year]
							 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
							 , SUM(Total_Amount_Per_Month) AS [Total Amount]
							 , SUM(Owned_Store) AS [No Of Store]
							 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
						FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@monthly)  
						WHERE Country = @Country					
					END
			END
		ELSE 
			IF @Device_Type != 'ALLOFALL'
				BEGIN
					INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
					SELECT YEAR(@monthly) AS [Year]
						 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
						 , SUM(Total_Amount_Per_Month) AS [Total Amount]
						 , SUM(Owned_Store) AS [No Of Store]
						 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
					FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@monthly)  
					WHERE Device_Type = @Device_Type				
				END
			 ELSE
				BEGIN
					INSERT INTO TempTable_DMC_Monthly_Revenue_By_Account_Type_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
					SELECT YEAR(@monthly) AS [Year]
						 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
						 , SUM(Total_Amount_Per_Month) AS [Total Amount]
						 , SUM(Owned_Store) AS [No Of Store]
						 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
					FROM dbo.DMC_Monthly_Subscription_By_Account_Type(@monthly)  			 
				END
	FETCH NEXT FROM record_cursor INTO @monthly
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Country_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_By_Country_Summary] @StartDate date, @EndDate date, @Country nvarchar(200)
AS
DECLARE @monthly as date

DECLARE record_cursor CURSOR
FOR 
	SELECT EOMONTH(MonthYearList) 
	FROM dbo.Get_MonthYearList(@StartDate, @EndDate)
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @monthly

-- Drop table if exists
DROP TABLE IF EXISTS [TempTable_DMC_Monthly_Revenue_By_Country_Summary]

-- Create the table
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_By_Country_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] int NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO TempTable_DMC_Monthly_Revenue_By_Country_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
		SELECT YEAR(@monthly) AS [Year]
			 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
			 , SUM(Total_Amount_Per_Month) AS [Total Amount]
			 , SUM(Owned_Store) AS [No Of Store]
			 , CAST(SUM(Total_Amount_Per_Month) / SUM(Owned_Store) AS decimal(10,2)) AS [Average]
		FROM dbo.DMC_Monthly_Subscription_By_Country(@monthly)  
		WHERE Country = @Country
	FETCH NEXT FROM record_cursor INTO @monthly
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_TempTable_DMC_Monthly_Revenue_Summary] @StartDate date, @EndDate date
AS
DECLARE @monthly as date

DECLARE record_cursor CURSOR
FOR 
	SELECT EOMONTH(MonthYearList)
	FROM dbo.Get_MonthYearList(@StartDate, @EndDate)
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @monthly

-- Drop table if exists
DROP TABLE IF EXISTS [TempTable_DMC_Monthly_Revenue_Summary]

-- Create the table
CREATE TABLE [dbo].[TempTable_DMC_Monthly_Revenue_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] int NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO TempTable_DMC_Monthly_Revenue_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
		SELECT YEAR(@monthly) AS [Year]
			 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
			 , SUM(Monthly_Fee) AS [Total Amount]
			 , COUNT(Store_No) AS [No Of Store]
			 , CAST(SUM(Monthly_Fee) / COUNT(Store_No) AS decimal(10,2)) AS [Average]
		FROM dbo.DMC_Monthly_Subscription(@monthly)  
	FETCH NEXT FROM record_cursor INTO @monthly
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_TempTable_Maintenance_Services_Monthly_Revenue_Summary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_TempTable_Maintenance_Services_Monthly_Revenue_Summary] @StartDate date, @EndDate date, @Services_Group nvarchar(1)
AS
DECLARE @monthly as date

DECLARE record_cursor CURSOR
FOR 
	SELECT EOMONTH(MonthYearList)
	FROM dbo.Get_MonthYearList(@StartDate, @EndDate)
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @monthly

-- Drop table if exists
DROP TABLE IF EXISTS [TempTable_Maintenance_Services_Monthly_Revenue_Summary]

-- Create the table
CREATE TABLE [dbo].[TempTable_Maintenance_Services_Monthly_Revenue_Summary](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Year] int NULL,
	[Month] [nvarchar](3) NULL,
	[Total_Amount] [money] NULL,
	[No_Of_Store] [int] NULL,
	[Average] [money] NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

WHILE @@FETCH_STATUS = 0
BEGIN
		INSERT INTO TempTable_Maintenance_Services_Monthly_Revenue_Summary([Year], [Month], [Total_Amount], [No_Of_Store], [Average])
		SELECT YEAR(@monthly) AS [Year]
			 , SUBSTRING(DATENAME(month, @monthly), 1, 3) AS [Month]
			 , SUM([Amount On Month]) AS [Total Amount]
			 , COUNT([Store ID]) AS [No Of Store]
			 --, CAST(AVG([Amount On Month]) AS decimal(10,2)) AS [Average] 
			 , CAST(SUM([Amount On Month]) / COUNT([Store ID]) AS decimal(10,2)) AS [Average]
		FROM dbo.Maintenance_Monthly_Revenue(@monthly, @Services_Group)
	FETCH NEXT FROM record_cursor INTO @monthly
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Termed_Licence_Renewal_Recovered_Invoice_Items]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Termed_Licence_Renewal_Recovered_Invoice_Items]
	@UID nvarchar(20),
	@PO_No nvarchar(50)

AS
DECLARE @Invoice_No nvarchar(30)
DECLARE @Invoice_Date nvarchar(30)
DECLARE @Item_Code nvarchar(50)
DECLARE @Chargeable int
DECLARE @Customer_ID nvarchar(20)

DECLARE @Licence_Code nvarchar(50)
DECLARE @Application_Type nvarchar(30)

DECLARE record_cursor CURSOR
FOR
	SELECT DISTINCT Invoice_No, Invoice_Date, CAST(Chargeable As int), Customer_ID
	FROM LMS_Termed_Licence_Renewal 
	WHERE Renewal_UID = @UID
OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Chargeable, @Customer_ID

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN	
		SET @Licence_Code = (SELECT Licence_Code FROM LMS_Termed_Licence_Renewal WHERE Renewal_UID = @UID)
		SET @Item_Code = (SELECT TOP 1 Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Bill items' AND Value_3 = (SELECT Application_Type FROM LMS_Licence WHERE Licence_Code = @Licence_Code) ORDER BY ID DESC)
		IF @Chargeable = 1
		BEGIN
			INSERT INTO DB_Recovered_Invoice(Invoice_No, Invoice_Date, Item_Code, Currency, Amount, Customer_ID, PO_No) 
			VALUES(@Invoice_No, CAST(@Invoice_Date AS date), @Item_Code, '', 0, @Customer_ID, @PO_No)
		END
	END
	FETCH NEXT FROM record_cursor INTO @Invoice_No, @Invoice_Date, @Chargeable, @Customer_ID
END
CLOSE record_cursor
DEALLOCATE record_cursor

GO
/****** Object:  StoredProcedure [dbo].[SP_Insert_Termed_Licence_Renewal_Staging]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Insert_Termed_Licence_Renewal_Staging]
	@Licence_Code nvarchar(50),
	@PO_No nvarchar(20),
	@PO_Date nvarchar(10),
	@Chargeable bit,
	@Currency nvarchar(5),
	@Fee money,
	@Remarks nvarchar(100),
	@Customer_ID nvarchar(20),
	@Sales_Representative_ID nvarchar(10)

AS
DECLARE @_PO_Date nvarchar(20)
DECLARE @_Invoice_No nvarchar(30)
DECLARE @_Invoice_Date nvarchar(20)

SET @_PO_Date = CASE WHEN @PO_No = 'NA' THEN NULL ELSE @PO_Date END
SET @_Invoice_No = CASE WHEN @PO_No = 'NA' THEN 'NA' ELSE '' END
SET @_Invoice_Date = NULL

BEGIN
	IF NOT EXISTS(SELECT * FROM LMS_Termed_Licence_Renewal_Staging WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Licence_Code = @Licence_Code)
		BEGIN
			INSERT INTO LMS_Termed_Licence_Renewal_Staging(Licence_Code, PO_No, PO_Date, Invoice_No, Invoice_Date, Renewal_Date, Chargeable, Currency, Fee, Remarks, Customer_ID, Sales_Representative_ID)
			VALUES(@Licence_Code, @PO_No, @_PO_Date, @_Invoice_No, @_Invoice_Date, GETDATE(), @Chargeable, @Currency, @Fee, @Remarks, @Customer_ID, @Sales_Representative_ID)
		END
	ELSE
		BEGIN
			UPDATE LMS_Termed_Licence_Renewal_Staging
			SET Licence_Code = @Licence_Code
			  , PO_No = @PO_No
			  , PO_Date = @_PO_Date
			  , Invoice_No = ''
			  , Invoice_Date = NULL
			  , Chargeable = @Chargeable
			  , Currency = @Currency
			  , Fee = @Fee
			  , Remarks = @Remarks
			WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No AND Licence_Code = @Licence_Code
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Licence_Key_Reset_Production]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Licence_Key_Reset_Production]
AS
BEGIN
    -- 01. Update the maxresetdate to end of current month
	-- hqid 98 is the hqid for PTTE
	-- storeid 493 is the store for parking license key for production use
	-- app_type 7 is the id for PC Scale type licence key

	UPDATE L_dmcmobiletoken 
	SET maxresetdate = REPLACE(CAST(EOMONTH(GETDATE()) AS nvarchar), '-', '') + '000000' 
	WHERE hqid = 98 
	  AND storeid = 493 
	  AND updates = 1 
	  AND status = 1 
	  AND app_type IN (7)
	  AND maxresetdate != REPLACE(CAST(EOMONTH(GETDATE()) AS nvarchar), '-', '') + '000000'

	-- 02. Update all license reset counter to 0
	UPDATE L_dmcmobiletoken 
	SET totalreset = 0 
	WHERE hqid = 98 
	  AND storeid = 493 
	  AND updates = 1 
	  AND app_type IN (7) 
	  AND totalreset > 0
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Module_Licence_Order]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Module_Licence_Order] 
	@Customer_ID nvarchar(20)

AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT * 
                    FROM (
					        SELECT DISTINCT + ', ' + QUOTENAME(Value_1) AS QueryColName
                            FROM DB_Lookup
				            WHERE Lookup_Name = 'Module Term Mapping'
					      ) TBL 
						  ORDER BY CASE QueryColName WHEN ', [e.Sense]' THEN 1 WHEN ', [BYOC]' THEN 2 ELSE 3 END
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT LMS_Module_Licence_Order.UID, PO_No AS [PO No]
                    , FORMAT(PO_Date, ''dd MMM yy'') AS [PO Date]
	                , CASE WHEN Is_Cancelled = 1 THEN ''CANCELLED'' ELSE Invoice_No END AS [Invoice No]
	                , FORMAT(Invoice_Date, ''dd MMM yy'') AS [Invoice Date]
	                , FORMAT(Created_Date, ''dd MMM yy'') AS [Created Date]
	                , Master_Sales_Representative.Name AS [Requested By]
	                , ' + @cols +
             ' FROM LMS_Module_Licence_Order
			   INNER JOIN Master_Sales_Representative ON Master_Sales_Representative.Sales_Representative_ID = LMS_Module_Licence_Order.Sales_Representative_ID
               INNER JOIN ( SELECT * FROM (SELECT UID, Module_Type, Quantity FROM LMS_Module_Licence_Order_Item ) AS SourceTable
							PIVOT
							(
								SUM(Quantity)
								FOR Module_Type IN (' + @cols + ')
							) AS PivotTable) I ON I.UID = LMS_Module_Licence_Order.UID WHERE Customer_ID = ''' + @Customer_ID + ''' ORDER BY Chargeable DESC, Created_Date DESC'

EXECUTE(@query)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_MonthlyRevenueSummary]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SP_MonthlyRevenueSummary]
  @ReportDate date,
  @Country nvarchar(100) = NULL,
  @DeviceType nvarchar(50) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  -- 1) load once
  SELECT *
  INTO   #src
  FROM   dbo.DMC_Monthly_Subscription_By_Account_Type_Base_USD(@ReportDate)

  -- 2) overall counts
  SELECT COUNT(DISTINCT Headquarter_ID) AS Headquarter_Count
	   , SUM(Owned_Store) AS Store_Count
	   , SUM(Total_Amount_Per_Month) AS Total_Amount
  FROM #src;

  -- 3) by Country
  SELECT COALESCE(Country,'Total') AS Country
       , SUM(Owned_Store) AS Stores
	   , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
	   , CAST(SUM(Total_Amount_Per_Month) / NULLIF(SUM(Owned_Store), 0) AS decimal(10,0)) AS Average
  FROM #src
  GROUP BY Country
  WITH ROLLUP
  ORDER BY CASE WHEN GROUPING(Country) = 1 THEN 1 ELSE 0 END, Country;

  -- 4) by Customer class
  SELECT COALESCE(Customer,'Total') AS Customer
       , SUM(Owned_Store) AS Stores
	   , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
	   , CAST(SUM(Total_Amount_Per_Month) / NULLIF(SUM(Owned_Store), 0) AS decimal(10,0)) AS Average
  FROM (
    SELECT CASE WHEN Headquarter_Name LIKE '%sushiro%' THEN 'Sushiro'
                WHEN Headquarter_Name LIKE '%chateraise%' THEN 'Chateraise'
                ELSE 'Others' END AS Customer
		 , Owned_Store
		 , Total_Amount_Per_Month
    FROM #src
  ) x
  GROUP BY Customer
  WITH ROLLUP
  ORDER BY CASE WHEN GROUPING(Customer) = 1 THEN 1 ELSE 0 END, Customer;

  -- 5) by Device_Type
  SELECT COALESCE(Device_Type,'Total') AS Segment
       , SUM(Owned_Store) AS Stores
	   , CAST(SUM(Total_Amount_Per_Month) AS decimal(10,0)) AS Total
	   , CAST(SUM(Total_Amount_Per_Month) / NULLIF(SUM(Owned_Store),0) AS decimal(10,0)) AS Average
  FROM #src
  GROUP BY Device_Type
  WITH ROLLUP
  ORDER BY CASE WHEN GROUPING(Device_Type) = 1 THEN 1 ELSE 0 END, Device_Type;
END

GO
/****** Object:  StoredProcedure [dbo].[SP_ReAssignBillEntity]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_ReAssignBillEntity]
	@Subscription_ID nvarchar(20),
	@Customer_ID nvarchar(20)

AS
BEGIN
	IF EXISTS(SELECT * FROM DB_Bill_Entity_Special_Arranged WHERE Subscription_ID = @Subscription_ID)
		BEGIN
			UPDATE DB_Bill_Entity_Special_Arranged
			SET Arranged_Bill_Entity = @Customer_ID
			WHERE Subscription_ID = @Subscription_ID
		END
	ELSE
		BEGIN
			INSERT INTO DB_Bill_Entity_Special_Arranged(Subscription_ID, Arranged_Bill_Entity)
			VALUES(@Subscription_ID, @Customer_ID)
		END
END
GO
/****** Object:  StoredProcedure [dbo].[SP_Reset_Staging_Table]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Reset_Staging_Table]
	@Table_Name nvarchar(50)
AS
BEGIN

IF @Table_Name = 'DMC_Subscription_Staging'	
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS DMC_Subscription_Staging

		CREATE TABLE [dbo].[DMC_Subscription_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Headquarter_ID] [nvarchar](20) NULL,
			[Store_ID] [nvarchar](20) NULL,
			[Synced_dmcstore_userstoreid] [nvarchar](10) NULL,
			[Duration] [nvarchar](20) NULL,
			[Currency] [nvarchar](5) NULL,
			[Fee] [money] NULL,
			[Payment_Method] [nvarchar](20) NULL,
			[Payment_Mode] [nvarchar](20) NULL,
			[Subscriber_Group] [nvarchar](1) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'LMS_Licence_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS LMS_Licence_Staging

		CREATE TABLE [dbo].[LMS_Licence_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Customer_ID] [nvarchar](20) NULL,
			[PO_No] [nvarchar](50) NULL,
			[Application_Type] [nvarchar](30) NULL,
			[OS_Type] [nvarchar](30) NULL,
			[Licence_Code] [nvarchar](50) NULL,
			[Email] [nvarchar](100) NULL,
			[Sales_Representative_ID] [nvarchar](10) NULL,
			[Chargeable] [bit] NULL,
			[Remarks] [nvarchar](100) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'LMS_Module_Licence_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS LMS_Module_Licence_Staging

		CREATE TABLE [dbo].[LMS_Module_Licence_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Customer_ID] [nvarchar](20) NULL,
			[PO_No] [nvarchar](50) NULL,
			[Module_Type] [nvarchar](20) NULL,
			[Quantity] [int] NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'LMS_AI_Licence_Renewal_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS LMS_AI_Licence_Renewal_Staging

		CREATE TABLE [dbo].[LMS_AI_Licence_Renewal_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Licence_Code] [nvarchar](50) NULL,
			[PO_No] [nvarchar](50) NULL,
			[PO_Date] [date] NULL,
			[Invoice_No] [nvarchar](30) NULL,
			[Invoice_Date] [date] NULL,
			[Renewal_Date] [date] NULL,
			[Chargeable] [bit] NULL,
			[Currency] [nvarchar](5) NULL,
			[Fee] [money] NULL,
			[Remarks] [nvarchar](100) NULL,
			[Customer_ID] [nvarchar](20) NULL,
			[Sales_Representative_ID] [nvarchar](10) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'LMS_Termed_Licence_Renewal_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS LMS_Termed_Licence_Renewal_Staging

		CREATE TABLE [dbo].[LMS_Termed_Licence_Renewal_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Licence_Code] [nvarchar](50) NULL,
			[PO_No] [nvarchar](50) NULL,
			[PO_Date] [date] NULL,
			[Invoice_No] [nvarchar](30) NULL,
			[Invoice_Date] [date] NULL,
			[Renewal_Date] [date] NULL,
			[Chargeable] [bit] NULL,
			[Currency] [nvarchar](5) NULL,
			[Fee] [money] NULL,
			[Remarks] [nvarchar](100) NULL,
			[Customer_ID] [nvarchar](20) NULL,
			[Sales_Representative_ID] [nvarchar](10) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'CZL_Account_Model_Upgrade_Charge_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS CZL_Account_Model_Upgrade_Charge_Staging

		CREATE TABLE [dbo].[CZL_Account_Model_Upgrade_Charge_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Upgraded_Model] [nvarchar](10) NULL,
			[PO_No] [nvarchar](50) NULL,
			[PO_Date] [date] NULL,
			[Invoice_No] [nvarchar](30) NULL,
			[Invoice_Date] [date] NULL,
			[Upgraded_Date] [date] NULL,
			[Chargeable] [bit] NULL,
			[Currency] [nvarchar](5) NULL,
			[Fee] [money] NULL,
			[Remarks] [nvarchar](100) NULL,
			[Customer_ID] [nvarchar](20) NULL,
			[Sales_Representative_ID] [nvarchar](10) NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'Maintenance_ESL_Tags_Deployment_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS Maintenance_ESL_Tags_Deployment_Staging

		CREATE TABLE [dbo].[Maintenance_ESL_Tags_Deployment_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Customer_ID] [nvarchar](20) NOT NULL,
			[Store_ID] [nvarchar](20) NOT NULL,
			[Tags_Group] [nvarchar](20) NOT NULL,
			[Tags_Type] [nvarchar](10) NOT NULL,
			[Quantity] [int] NOT NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
ELSE IF @Table_Name = 'Maintenance_Contract_Product_Line_Items_Staging'
	BEGIN
		-- Check if the table exists, if yes, drop it.
		DROP TABLE IF EXISTS Maintenance_Contract_Product_Line_Items_Staging

		CREATE TABLE [dbo].[Maintenance_Contract_Product_Line_Items_Staging](
			[ID] [int] IDENTITY(1,1) NOT NULL,
			[Product_Unique_ID] [nvarchar](20) NOT NULL,
			[Serial_No] [nvarchar](20) NOT NULL,
			[Product_Name] [nvarchar](100) NOT NULL,
			[Base_Currency] [nvarchar](3) NOT NULL,
			[Base_Currency_Value] [money] NOT NULL,
			[Maintenance_Cost] [money] NOT NULL,
		PRIMARY KEY CLUSTERED 
		(
			[ID] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Server_Space_Consumption_Month]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Server_Space_Consumption_Month] 
AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT ', ISNULL(' + QUOTENAME(LEFT(DATENAME(month, DATEADD(month, [Number] - 1, '19000101')), 3)) + ', 0) AS ' + QUOTENAME(LEFT(DATENAME(month, DATEADD(month, [Number] - 1, '19000101')), 3)) AS QueryColName
                    FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                    WHERE Number <= 12
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT ', ' + QUOTENAME(LEFT(DATENAME(month, DATEADD(month, [Number] - 1, '19000101')), 3)) AS QueryColName 
                      FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                      WHERE Number <= 12
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT [Year]
                    , [COL]
	                , ' + @cols +
             ' FROM ( SELECT [Year], [Month], COL, VAL FROM _Server_Space_Month_Growth
				      CROSS APPLY ( VALUES(''Server Space'', [Server Space]), (''Used'', [Used]), (''Used Growth'', [Used Growth]), (''Available'', [Available]), (''Avail Diff'', [Avail Diff]), (''Usage'', [Usage]), (''Usage Diff'', [Usage Diff]), (''DB Size'', [DB Size]), (''DB Growth'', [DB Growth]) ) CS (COL, VAL)) T
				      PIVOT (MAX([VAL]) FOR [Month] IN (' + @PVcols + ')
			        ) PVT 
			   ORDER BY [Year] DESC, CASE [Col] WHEN ''Used'' THEN 1 WHEN ''Used Growth'' THEN 2 WHEN ''Available'' THEN 3 WHEN ''Avail Diff'' THEN 4 WHEN ''Usage'' THEN 5 WHEN ''Usage Diff'' THEN 6 WHEN ''DB Size'' THEN 7 WHEN ''DB Growth'' THEN 8 ELSE 9 END '

EXECUTE(@query)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Server_Space_Consumption_Quarter]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Server_Space_Consumption_Quarter] 
AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT ', ISNULL(' + QUOTENAME([Number]) + ', 0) AS ' + QUOTENAME([Number]) AS QueryColName 
                    FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                    WHERE Number <= 4
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT ', ' + QUOTENAME([Number]) AS QueryColName 
                      FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                      WHERE Number <= 4
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT [Year]
                    , [COL]
	                , ' + @cols +
             ' FROM ( SELECT [Year], [Quarter], COL, VAL FROM _Server_Space_Quarter_Growth
				      CROSS APPLY ( VALUES(''Server Space'', [Server Space]), (''Used'', [Used]), (''Used Growth'', [Used Growth]), (''Available'', [Available]), (''Avail Diff'', [Avail Diff]), (''Usage'', [Usage]), (''Usage Diff'', [Usage Diff]), (''DB Size'', [DB Size]), (''DB Growth'', [DB Growth]) ) CS (COL, VAL)) T
				      PIVOT (MAX([VAL]) FOR [Quarter] IN (' + @PVcols + ')
			        ) PVT 
			   ORDER BY [Year] DESC, CASE [Col] WHEN ''Used'' THEN 1 WHEN ''Used Growth'' THEN 2 WHEN ''Available'' THEN 3 WHEN ''Avail Diff'' THEN 4 WHEN ''Usage'' THEN 5 WHEN ''Usage Diff'' THEN 6 WHEN ''DB Size'' THEN 7 WHEN ''DB Growth'' THEN 8 ELSE 9 END '

EXECUTE(@query)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Server_Space_Consumption_Semiannual]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Server_Space_Consumption_Semiannual] 
AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT ', ISNULL(' + QUOTENAME([Number]) + ', 0) AS ' + QUOTENAME([Number]) AS QueryColName 
                    FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                    WHERE Number <= 2
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT ', ' + QUOTENAME([Number]) AS QueryColName 
                      FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                      WHERE Number <= 2
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT [Year]
                    , [COL]
	                , ' + @cols +
             ' FROM ( SELECT [Year], [Semiannual], COL, VAL FROM _Server_Space_Semiannual_Growth
				      CROSS APPLY ( VALUES(''Server Space'', [Server Space]), (''Used'', [Used]), (''Used Growth'', [Used Growth]), (''Available'', [Available]), (''Avail Diff'', [Avail Diff]), (''Usage'', [Usage]), (''Usage Diff'', [Usage Diff]), (''DB Size'', [DB Size]), (''DB Growth'', [DB Growth]) ) CS (COL, VAL)) T
				      PIVOT (MAX([VAL]) FOR [Semiannual] IN (' + @PVcols + ')
			        ) PVT 
			   ORDER BY [Year] DESC, CASE [Col] WHEN ''Used'' THEN 1 WHEN ''Used Growth'' THEN 2 WHEN ''Available'' THEN 3 WHEN ''Avail Diff'' THEN 4 WHEN ''Usage'' THEN 5 WHEN ''Usage Diff'' THEN 6 WHEN ''DB Size'' THEN 7 WHEN ''DB Growth'' THEN 8 ELSE 9 END '

EXECUTE(@query)


END
GO
/****** Object:  StoredProcedure [dbo].[SP_Server_Space_Consumption_Week]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Server_Space_Consumption_Week] 
AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT ', ISNULL(' + QUOTENAME([Number]) + ', 0) AS ' + QUOTENAME([Number]) AS QueryColName 
                    FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                    WHERE Number <= 53
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT ', ' + QUOTENAME([Number]) AS QueryColName 
                      FROM ( SELECT ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS [Number] FROM master.dbo.spt_values
                         ) TBL
                      WHERE Number <= 53
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT [Year]
                    , [COL]
	                , ' + @cols +
             ' FROM ( SELECT [Year], [Week No], COL, VAL FROM _Server_Space_Week_Growth
				      CROSS APPLY ( VALUES(''Server Space'', [Server Space]), (''Used'', [Used]), (''Used Growth'', [Used Growth]), (''Available'', [Available]), (''Avail Diff'', [Avail Diff]), (''Usage'', [Usage]), (''Usage Diff'', [Usage Diff]), (''DB Size'', [DB Size]), (''DB Growth'', [DB Growth]) ) CS (COL, VAL)) T
				      PIVOT (MAX([VAL]) FOR [Week No] IN (' + @PVcols + ')
			        ) PVT 
			   ORDER BY [Year] DESC, CASE [Col] WHEN ''Used'' THEN 1 WHEN ''Used Growth'' THEN 2 WHEN ''Available'' THEN 3 WHEN ''Avail Diff'' THEN 4 WHEN ''Usage'' THEN 5 WHEN ''Usage Diff'' THEN 6 WHEN ''DB Size'' THEN 7 WHEN ''DB Growth'' THEN 8 ELSE 9 END '

EXECUTE(@query)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Server_Space_Consumption_Year]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Server_Space_Consumption_Year] 
AS
BEGIN
DECLARE @cols AS nvarchar(MAX)
DECLARE @PVcols AS nvarchar(MAX)
DECLARE @query AS nvarchar(MAX)

SET @cols = STUFF(( SELECT [Year] FROM ( SELECT DISTINCT ', ISNULL(' + QUOTENAME([YEAR]) + ', 0) AS ' + QUOTENAME([YEAR]) AS [Year]
						                    FROM DB_List_Of_Year
											WHERE [Year] > YEAR(GETDATE()) - 5
						                  ) TBL 
					   ORDER BY [YEAR]
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @PVcols = STUFF(( SELECT [Year] FROM ( SELECT DISTINCT ', ' + QUOTENAME([YEAR]) AS [Year]
						                    FROM DB_List_Of_Year
											WHERE [Year] > YEAR(GETDATE()) - 5
						                  ) TBL 
					   ORDER BY [YEAR]
                   FOR XML PATH(''), TYPE
                  ).value('.', 'NVARCHAR(MAX)') 
                  ,1,1,'')

SET @query = ' SELECT COL, ' + @cols +
             ' FROM (
		              SELECT [Year], COL, VAL FROM _Server_Space_Year_Growth
		              CROSS APPLY ( VALUES(''Server Space'', [Server Space]), (''Used'', [Used]), (''Used Growth'', [Used Growth]), (''Available'', [Available]), (''Avail Diff'', [Avail Diff]), (''Usage'', [Usage]), (''Usage Diff'', [Usage Diff]), (''DB Size'', [DB Size]), (''DB Growth'', [DB Growth]) ) CS (COL, VAL)
	                ) T
               PIVOT 
               (
                  MAX([VAL])
                  FOR [Year] IN (' + @PVcols + ')
               ) PVT 
			   ORDER BY CASE [Col] WHEN ''Used'' THEN 1 WHEN ''Used Growth'' THEN 2 WHEN ''Available'' THEN 3 WHEN ''Avail Diff'' THEN 4 WHEN ''Usage'' THEN 5 WHEN ''Usage Diff'' THEN 6 WHEN ''DB Size'' THEN 7 WHEN ''DB Growth'' THEN 8 ELSE 9 END '

EXECUTE(@query)


END
GO
/****** Object:  StoredProcedure [dbo].[SP_Suspend_Store]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Suspend_Store]

AS
DECLARE @hqid int
DECLARE @storeid int
DECLARE @name nvarchar(100)
DECLARE @Store_ID nvarchar(50)

DECLARE @ActiveStore int
DECLARE @Reason nvarchar(200)

DECLARE record_cursor CURSOR
FOR
	SELECT hqid, storeid, name, [Store ID], Reason
	FROM vw_Current_Expired_Store
	--WHERE hqid NOT IN (103, 153, 160, 204, 210, 232, 247, 248, 256, 258, 259)   --exclude stores of these hq from being suspended
	ORDER BY hqid, storeid

OPEN record_cursor
FETCH NEXT FROM record_cursor INTO @hqid, @storeid, @name, @Store_ID, @Reason

WHILE @@FETCH_STATUS = 0
	BEGIN	
			-- 01. Insert records into suspended_store table
			INSERT INTO Suspended_Store(Store_ID, Store_Name, Suspended_Date, Reason)
			--VALUES(@Store_ID, @name, GETDATE(), @Reason)
			VALUES(@Store_ID, @name, (SELECT DATEADD(DAY, 1, MAX(End_Date)) FROM DMC_Subscription WHERE Store_ID = @Store_ID), @Reason)

			-- 02. Suspend store in dmcstore table
			--UPDATE OPENQUERY(DMCLIVE, 'SELECT * FROM dmcstore')
			--SET acccounttype = 2, updates = 2
			--WHERE hqid = @hqid AND id = @storeid

			-- 03. Log the suspended store of dmcstore
			SELECT hqid, id, name, updates, acccounttype
			FROM OPENQUERY(DMCLIVE, 'SELECT * FROM dmcstore')
			WHERE hqid = @hqid AND id = @storeid

			-- 04. Update store status in DMC_Store to Suspended
			UPDATE DMC_Store
			--SET Is_Active = 0, Inactive_Date = DATEADD(DAY, -1, GETDATE())
			SET Is_Active = 0, Inactive_Date = (SELECT MAX(End_Date) FROM DMC_Subscription WHERE Store_ID = @Store_ID)
			WHERE Store_ID IN (@Store_ID)

			-- 05. Suspend User in DMC_User if all stores in DMC_Store of a HQ store pool have been suspended
			SET @ActiveStore = (SELECT COUNT(*) FROM DMC_Store WHERE Is_Active = 1 AND Headquarter_ID IN (FORMAT(@hqid, 'D6')))
			If @ActiveStore <= 0
				BEGIN
					UPDATE DMC_User
					SET Is_Active = 0, Inactive_Date = DATEADD(DAY, -1, GETDATE())
					WHERE Headquarter_ID = FORMAT(@hqid, 'D6')

					-- 06. Log the suspended user
					SELECT * FROM DMC_User WHERE Headquarter_ID = FORMAT(@hqid, 'D6')
				END

			FETCH NEXT FROM record_cursor INTO @hqid, @storeid, @name, @Store_ID, @Reason
	END
CLOSE record_cursor
DEALLOCATE record_cursor
GO
/****** Object:  StoredProcedure [dbo].[SP_Sync_LMS_Licence]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Sync_LMS_Licence]
AS

BEGIN
	-- 01. Update LMSPortal LMS_Licence table based on DMC dmcmobiletoken table
	UPDATE LMS_Licence
	SET LMS_Licence.Synced_dmcmobiletoken_unique_id = UPPER(L_dmcmobiletoken.unique_id)
	  , LMS_Licence.Synced_dmcmobiletoken_activateddate = CAST(SUBSTRING(CAST(L_dmcmobiletoken.activateddate AS nvarchar), 1, 8) AS date)
	  , LMS_Licence.Synced_dmcmobiletoken_expireddate = CASE WHEN L_dmcmobiletoken.status = 0 THEN NULL ELSE CASE WHEN L_dmcmobiletoken.expireddate < 99999999 THEN CAST(CAST(L_dmcmobiletoken.expireddate AS nvarchar) AS date) ELSE '2999-12-31' END END
	  , LMS_Licence.Synced_dmcmobiletoken_status = CASE L_dmcmobiletoken.status WHEN 0 THEN 'New' WHEN 1 THEN 'Activated' WHEN 2 THEN 'Renew' WHEN 3 THEN 'Blocked' END
	  , LMS_Licence.Synced_dmcmobiletoken_term = L_dmcmobiletoken.term
	  , LMS_Licence.Synced_dmcmobiletoken_maxhq = L_dmcmobiletoken.maxhq
	  , LMS_Licence.Synced_dmcmobiletoken_maxstore = L_dmcmobiletoken.maxstore
	FROM LMS_Licence, L_dmcmobiletoken
	WHERE REPLACE(LMS_Licence.Licence_Code, '-', '') = L_dmcmobiletoken.license_code
      AND (
             LMS_Licence.Synced_dmcmobiletoken_status <> CASE L_dmcmobiletoken.status 
                                                              WHEN 0 THEN 'New' 
                                                              WHEN 1 THEN 'Activated' 
                                                              WHEN 2 THEN 'Renew' 
                                                              WHEN 3 THEN 'Blocked' 
                                                         END
             OR LMS_Licence.Synced_dmcmobiletoken_status IS NULL  
			 OR ( 
					 LMS_Licence.Synced_dmcmobiletoken_status = CASE L_dmcmobiletoken.status 
																	  WHEN 0 THEN 'New' 
																	  WHEN 1 THEN 'Activated' 
																	  WHEN 2 THEN 'Renew' 
																	  WHEN 3 THEN 'Blocked' 
																 END
					 AND ( ISNULL(LMS_Licence.Synced_dmcmobiletoken_expireddate, '1900-01-01') <> ISNULL( CASE WHEN L_dmcmobiletoken.status = 0 
					                                                                                           THEN NULL 
                                                                                                               ELSE CASE WHEN L_dmcmobiletoken.expireddate < 99999999 
                                                                                                                         THEN CAST(CAST(L_dmcmobiletoken.expireddate AS nvarchar) AS date)
                                                                                                                         ELSE '2999-12-31' 
                                                                                                                         END
                                                                                                               END, '1900-01-01')
						 )
				)
			 )


	-- 02. Update LMSPortal LMS_Licence table's Serial_No, AI_Device_ID and AI_Device_Serial_No column based on MAC address in CZL_Licenced_Devices
	UPDATE LMS_Licence
	SET LMS_Licence.Serial_No = CZL_Licenced_Devices.Scale_SN
	  , LMS_Licence.AI_Device_ID = CZL_Licenced_Devices.Device_ID
      , LMS_Licence.AI_Device_Serial_No = CZL_Licenced_Devices.Device_Serial
	FROM LMS_Licence, CZL_Licenced_Devices
	WHERE LMS_Licence.Synced_dmcmobiletoken_status = 'Activated'
	  AND UPPER(TRIM(LMS_Licence.Synced_dmcmobiletoken_unique_id)) = UPPER(TRIM(CZL_Licenced_Devices.MAC_Addr))
      AND LMS_Licence.Serial_No IS NULL
      AND LMS_Licence.AI_Device_ID IS NULL
      AND LMS_Licence.AI_Device_Serial_No IS NULL
	  AND LMS_Licence.Licence_Code IN (SELECT [Licence Code] FROM _LMS_Licence_Details WHERE [Application Type] LIKE '%AI%')
	  AND LMS_Licence.Licence_Code NOT IN (SELECT Value_1 FROM DB_Lookup WHERE Lookup_Name = 'Production Used Licence Key')
	  AND LMS_Licence.Customer_ID NOT IN ('CTR-000005', 'CTR-000121', 'CTR-000081')


	-- 03. Update LMSPortal DMC_Store Name, FTP_User, FTP_Password and Synced_dmcstore_id based on DMC dmcstore table
	-----  This to work with note that L_dmcstore.id cannot be deleted
	UPDATE DMC_Store
	SET Name = L_dmcstore.name
	  , FTP_User = L_dmcstore.ftpuser
	  , FTP_Password = L_dmcstore.ftppass
	  , Synced_dmcstore_saleslastuseddate = CAST(SUBSTRING(CAST(L_dmcstore.saleslastuseddate AS nvarchar), 1, 8) AS date)
	FROM DMC_Store
	LEFT JOIN L_dmcstore ON L_dmcstore.id = DMC_Store.Synced_dmcstore_id
	WHERE (
		        L_dmcstore.name <> DMC_Store.Name
		     OR L_dmcstore.ftpuser <> DMC_Store.FTP_User
		     OR L_dmcstore.ftppass <> DMC_Store.FTP_Password
		     OR CAST(SUBSTRING(CAST(L_dmcstore.saleslastuseddate AS nvarchar), 1, 8) AS date) <> DMC_Store.Synced_dmcstore_saleslastuseddate
		     OR DMC_Store.Name IS NULL
		     OR DMC_Store.FTP_User IS NULL
		     OR DMC_Store.FTP_Password IS NULL
		     OR DMC_Store.Synced_dmcstore_saleslastuseddate IS NULL
	)


	-- 04. Update LMSPortal DMC_User Table based on DMC dmcuser table
	UPDATE DMC_User
	SET DMC_User.Synced_dmcuser_devicetype = L_dmcuser.devicetype
	FROM DMC_User, L_dmcuser
	WHERE DMC_User.Username = L_dmcuser.username AND DMC_User.Synced_dmcuser_devicetype != L_dmcuser.devicetype



	-- 05. Drop and re-create LMS_Module_Licence_Activated table, this table store module licence type a particular key activate
	DROP TABLE IF EXISTS [LMS_Module_Licence_Activated]

	CREATE TABLE [dbo].[LMS_Module_Licence_Activated](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Licence_Code] [nvarchar](100) NULL,
		[Activated_Module_Type] [nvarchar](500) NULL,
	PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]


	-- Insert dmclicensemoduleassign record into local LMS_Module_Licence_Activated table
	INSERT INTO LMS_Module_Licence_Activated(Licence_Code, Activated_Module_Type)
	SELECT dmcmobiletokenlicensecode AS Licence_Code
		 , STRING_AGG( ( CASE dmcapimoduletypename WHEN 'esense' THEN 'e.Sense' WHEN 'nfidtare' THEN 'BYOC' WHEN 'ai' THEN 'AI' END) , ', ') WITHIN GROUP (ORDER BY CASE dmcapimoduletypename WHEN 'esense' THEN 1 WHEN 'nfidtare' THEN 2 ELSE 3 END ) AS Module_Type
	FROM L_dmclicensemoduleassign
	WHERE dmcmobiletokenlicensecode NOT IN (SELECT Licence_Code FROM LMS_Module_Licence_Activated)
	GROUP BY dmcmobiletokenlicensecode



	-- 06. Drop and re-create LMS_Module_Licence_Pool table, this table store licence pool of each customer
	DROP TABLE IF EXISTS [LMS_Module_Licence_Pool]

	CREATE TABLE [dbo].[LMS_Module_Licence_Pool](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Headquarter_ID] [nvarchar](20) NULL,
		[Headquarter_Name] [nvarchar](100) NULL,
		[Synced_dmcstore_storeid] [nvarchar](5) NULL,
		[Store_No] [nvarchar](5) NULL,
		[Store_Name] [nvarchar](100) NULL,
		[Module_Type] [nvarchar](20) NULL,
		[Balance] [int] NULL,
		[Used] [int] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	-- Insert L_dmcmodulelicensepool record into local LMS_Module_Licence_Pool table
	--SELECT * FROM L_dmcmodulelicensepool
	INSERT INTO LMS_Module_Licence_Pool(Headquarter_ID, Headquarter_Name, Synced_dmcstore_storeid, Store_No, Store_Name, Module_Type, Balance, Used)
	SELECT hqid, [HQ Name], storeid, [Store Code], [Store Name], [Module Type], Balance, Used FROM L_dmcmodulelicensepool


	-- 07. Drop and re-create LMS_AI_Gateway_Licence table
	DROP TABLE IF EXISTS [LMS_AI_Gateway_Licence]

	CREATE TABLE [dbo].[LMS_AI_Gateway_Licence](
		[ID] [int] IDENTITY(1,1) NOT NULL,
		[Licence_Code] [nvarchar](50) NULL,
		[Synced_dmclicensetoken_Token] [nvarchar](50) NULL,
		[Synced_dmclicensetoken_createdate] [date] NULL,
		[Synced_dmclicensetoken_activatedate] [date] NULL,
		[Synced_dmclicensetoken_expiredate] [date] NULL,
	PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	--Insert AI Gateway Licence Token to local table 
	INSERT INTO LMS_AI_Gateway_Licence (Licence_Code, Synced_dmclicensetoken_Token, Synced_dmclicensetoken_createdate, Synced_dmclicensetoken_activatedate, Synced_dmclicensetoken_expiredate)
	SELECT licensecode
	     , token
	     , CAST(SUBSTRING(CAST(createdate AS nvarchar), 1, 8) AS date)
		 , CAST(SUBSTRING(CAST(activatedate AS nvarchar), 1, 8) AS date)
	     , DATEADD(D, -1, expireddate)
	FROM L_dmclicensetoken
    WHERE licensecode + token NOT IN (SELECT licence_code + synced_dmclicensetoken_token FROM LMS_AI_Gateway_Licence)

END
GO
/****** Object:  StoredProcedure [dbo].[SP_Void_DMC_Subscription]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Void_DMC_Subscription]
		@Subscription_ID nvarchar(20)
AS
BEGIN
	IF EXISTS (SELECT * FROM DMC_Subscription WHERE Subscription_ID = @Subscription_ID)
		BEGIN
			-- Update DMC_Subscription table 
			UPDATE DMC_Subscription SET Payment_Status = 'Cancelled'
            WHERE Subscription_ID = @Subscription_ID
		END
END

GO
/****** Object:  StoredProcedure [dbo].[SP_Void_Licence_Request]    Script Date: 13/6/2025 8:49:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SP_Void_Licence_Request]
		@PO_No nvarchar(50),
		@Customer_ID nvarchar(20)
AS
BEGIN
    -- Start the transaction
    BEGIN TRANSACTION
		BEGIN TRY
			IF EXISTS (SELECT * FROM LMS_Licence WHERE PO_No = @PO_No AND Customer_ID = @Customer_ID AND Is_Cancelled = 0)
				BEGIN
					-- 01. Update LMS_Licence table 
					UPDATE LMS_Licence SET Is_Cancelled = 1
					WHERE PO_No = @PO_No AND Customer_ID = @Customer_ID

					-- 02. If it is Module licence order, then update LMS_Module_Licence_Order table too
					IF EXISTS (SELECT * FROM LMS_Module_Licence_Order WHERE PO_No = @PO_No AND Customer_ID = @Customer_ID AND Is_Cancelled = 0)
					BEGIN
						UPDATE LMS_Module_Licence_Order SET Is_Cancelled = 1 
						WHERE PO_No = @PO_No AND Customer_ID = @Customer_ID
					END

					-- 03. Delete the PO No record from table DB_SO_No_By_PO if it is exists
					IF EXISTS (SELECT * FROM DB_SO_No_By_PO WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No)
					BEGIN 
						DELETE FROM DB_SO_No_By_PO WHERE Customer_ID = @Customer_ID AND PO_No = @PO_No
					END
				END
			-- Commit the transaction if everything is successful
			COMMIT TRANSACTION
    END TRY
    BEGIN CATCH
        -- Rollback the transaction if there is an error
        ROLLBACK TRANSACTION;

        -- Raise an error with the details of the exception
        DECLARE @ErrorMessage NVARCHAR(4000), @ErrorSeverity INT, @ErrorState INT;
        SELECT 
            @ErrorMessage = ERROR_MESSAGE(),
            @ErrorSeverity = ERROR_SEVERITY(),
            @ErrorState = ERROR_STATE();
        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END

GO
USE [master]
GO
ALTER DATABASE [LMSPortal] SET  READ_WRITE 
GO
