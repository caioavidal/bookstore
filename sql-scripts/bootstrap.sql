-- --------------------------------------------------------
-- Servidor:                     localhost
-- Versão do servidor:           PostgreSQL 15.3 (Debian 15.3-1.pgdg120+1) on x86_64-pc-linux-gnu, compiled by gcc (Debian 12.2.0-14) 12.2.0, 64-bit
-- OS do Servidor:               
-- HeidiSQL Versão:              12.4.0.6659
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES  */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


--CREATE DATABASE BookStoreDB;

--CREATE SCHEMA PUBLIC;

GRANT ALL PRIVILEGES ON SCHEMA public TO postgres;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO postgres;

-- Copiando estrutura para tabela public.users
CREATE TABLE IF NOT EXISTS "users" (
	"id" UUID NOT NULL,
	"name" VARCHAR(255) NOT NULL,
	"birth_date" TIMESTAMP NOT NULL,
	"email" VARCHAR(255) NOT NULL,
	"password_hash" TEXT NULL DEFAULT NULL,
	"role" VARCHAR(50) NULL DEFAULT NULL,
	"salt" BYTEA NULL DEFAULT NULL,
	PRIMARY KEY ("id"),
	UNIQUE ("email")
);

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela public.books
CREATE TABLE IF NOT EXISTS "books" (
	"id" UUID NOT NULL,
	"title" VARCHAR(250) NOT NULL,
	"author" VARCHAR(100) NOT NULL,
	"isbn" VARCHAR(20) NOT NULL,
	"publication_year" INTEGER NOT NULL,
	"price" NUMERIC(10,2) NOT NULL,
	PRIMARY KEY ("id")
);

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela public.inventory
CREATE TABLE IF NOT EXISTS "inventory" (
	"id" UUID NOT NULL,
	"book_id" UUID NULL DEFAULT NULL,
	"quantity" INTEGER NOT NULL,
	"minimum_quantity" INTEGER NOT NULL,
	"maximum_quantity" INTEGER NOT NULL,
	PRIMARY KEY ("id"),
	CONSTRAINT "inventory_book_id_fkey" FOREIGN KEY ("book_id") REFERENCES "books" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela public.orders
CREATE TABLE IF NOT EXISTS "orders" (
	"id" UUID NOT NULL,
	"user_id" UUID NOT NULL,
	"created_at" TIMESTAMP NOT NULL,
	PRIMARY KEY ("id"),
	CONSTRAINT "orders_user_id_fkey" FOREIGN KEY ("user_id") REFERENCES "users" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela public.order_items
CREATE TABLE IF NOT EXISTS "order_items" (
	"id" UUID NOT NULL,
	"order_id" UUID NOT NULL,
	"book_id" UUID NOT NULL,
	"price" NUMERIC(10,2) NOT NULL,
	"quantity" INTEGER NOT NULL,
	PRIMARY KEY ("id"),
	CONSTRAINT "order_items_book_id_fkey" FOREIGN KEY ("book_id") REFERENCES "books" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT "order_items_order_id_fkey" FOREIGN KEY ("order_id") REFERENCES "orders" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Exportação de dados foi desmarcado.

-- Copiando estrutura para tabela public.order_statuses
CREATE TABLE IF NOT EXISTS "order_statuses" (
	"id" UUID NOT NULL,
	"order_id" UUID NOT NULL,
	"order_state" INTEGER NOT NULL,
	"created_at" TIMESTAMP NOT NULL,
	PRIMARY KEY ("id"),
	CONSTRAINT "order_statuses_order_id_fkey" FOREIGN KEY ("order_id") REFERENCES "orders" ("id") ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Exportação de dados foi desmarcado.

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
