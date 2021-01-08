USE [master]
GO
/****** Object:  Database [projekt_laser]    Script Date: 09.01.2020 10:46:42 ******/
CREATE DATABASE [projekt_laser]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'projekt_laser', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\projekt_laser.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'projekt_laser_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.SQLEXPRESS\MSSQL\DATA\projekt_laser_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [projekt_laser] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [projekt_laser].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [projekt_laser] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [projekt_laser] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [projekt_laser] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [projekt_laser] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [projekt_laser] SET ARITHABORT OFF 
GO
ALTER DATABASE [projekt_laser] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [projekt_laser] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [projekt_laser] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [projekt_laser] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [projekt_laser] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [projekt_laser] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [projekt_laser] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [projekt_laser] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [projekt_laser] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [projekt_laser] SET  DISABLE_BROKER 
GO
ALTER DATABASE [projekt_laser] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [projekt_laser] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [projekt_laser] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [projekt_laser] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [projekt_laser] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [projekt_laser] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [projekt_laser] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [projekt_laser] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [projekt_laser] SET  MULTI_USER 
GO
ALTER DATABASE [projekt_laser] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [projekt_laser] SET DB_CHAINING OFF 
GO
ALTER DATABASE [projekt_laser] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [projekt_laser] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [projekt_laser] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [projekt_laser] SET QUERY_STORE = OFF
GO
USE [projekt_laser]
GO
/****** Object:  Table [dbo].[LASER]    Script Date: 09.01.2020 10:46:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LASER](
	[ID_KONFIGURACJI] [int] IDENTITY(1,1) NOT NULL,
	[MOC_LASERA] [int] NOT NULL,
	[PREDKOSC] [int] NOT NULL,
	[LICZBA_PRZEJSC] [int] NOT NULL,
 CONSTRAINT [PK_LASER] PRIMARY KEY CLUSTERED 
(
	[ID_KONFIGURACJI] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OBRAZ]    Script Date: 09.01.2020 10:46:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OBRAZ](
	[ID_OBRAZU] [int] IDENTITY(1,1) NOT NULL,
	[OBRAZ] [varbinary](max) NOT NULL,
	[ID_KONFIGURACJI] [int] NOT NULL,
 CONSTRAINT [PK_OBRAZ] PRIMARY KEY CLUSTERED 
(
	[ID_OBRAZU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OBRAZY]    Script Date: 09.01.2020 10:46:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OBRAZY](
	[ID_OBRAZU] [int] IDENTITY(1,1) NOT NULL,
	[OBRAZ] [varbinary](max) NOT NULL,
	[ID_KONFIGURACJI] [int] NOT NULL,
 CONSTRAINT [PK_OBRAZY_1] PRIMARY KEY CLUSTERED 
(
	[ID_OBRAZU] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[OBRAZ]  WITH CHECK ADD  CONSTRAINT [FK_OBRAZ_LASER] FOREIGN KEY([ID_KONFIGURACJI])
REFERENCES [dbo].[LASER] ([ID_KONFIGURACJI])
GO
ALTER TABLE [dbo].[OBRAZ] CHECK CONSTRAINT [FK_OBRAZ_LASER]
GO
ALTER TABLE [dbo].[OBRAZY]  WITH CHECK ADD  CONSTRAINT [FK_OBRAZY_LASER] FOREIGN KEY([ID_KONFIGURACJI])
REFERENCES [dbo].[LASER] ([ID_KONFIGURACJI])
GO
ALTER TABLE [dbo].[OBRAZY] CHECK CONSTRAINT [FK_OBRAZY_LASER]
GO
USE [master]
GO
ALTER DATABASE [projekt_laser] SET  READ_WRITE 
GO
