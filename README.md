--Creacion de tablas:

--Clientes:

CREATE TABLE [dbo].[Clientes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](30) NULL,
	[Apellido] [varchar](30) NULL,
	[TipoDocumento] [varchar](20) NULL,
	[Documento] [varchar](30) NULL,
 CONSTRAINT [PK_Clientes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Clientes_Documento] UNIQUE NONCLUSTERED 
(
	[Documento] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Productos:

CREATE TABLE [dbo].[Productos](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [varchar](10) NULL,
	[Descripcion] [varchar](30) NULL,
	[UnidadMedida] [varchar](10) NULL,
	[Precio] [int] NULL,
	[Stock] [int] NULL,
 CONSTRAINT [PK_Productos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

--Pedidos:

CREATE TABLE [dbo].[Pedidos](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[id_cliente] [int] NULL,
	[total] [int] NULL,
 CONSTRAINT [PK_Pedidos] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Pedidos]  WITH CHECK ADD  CONSTRAINT [FK_Pedidos_Clientes] FOREIGN KEY([id_cliente])
REFERENCES [dbo].[Clientes] ([ID])
GO

ALTER TABLE [dbo].[Pedidos] CHECK CONSTRAINT [FK_Pedidos_Clientes]
GO

--Pedidos Detalle:

CREATE TABLE [dbo].[Pedidos_detalle](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[id_pedido] [int] NOT NULL,
	[id_producto] [int] NULL,
	[cant] [int] NULL,
	[precio] [int] NULL,
 CONSTRAINT [PK_Pedidos_detalle] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Pedidos_detalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidosDet_Pedidos] FOREIGN KEY([id_pedido])
REFERENCES [dbo].[Pedidos] ([ID])
GO

ALTER TABLE [dbo].[Pedidos_detalle] CHECK CONSTRAINT [FK_PedidosDet_Pedidos]
GO

ALTER TABLE [dbo].[Pedidos_detalle]  WITH CHECK ADD  CONSTRAINT [FK_PedidosDet_Producto] FOREIGN KEY([id_producto])
REFERENCES [dbo].[Productos] ([ID])
GO

ALTER TABLE [dbo].[Pedidos_detalle] CHECK CONSTRAINT [FK_PedidosDet_Producto]
GO
