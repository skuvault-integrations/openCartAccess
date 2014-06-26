﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LINQtoCSV;
using NUnit.Framework;
using OpenCartAccess;
using OpenCartAccess.Models.Configuration;
using OpenCartAccess.Models.Product;

namespace OpenCartAccessTests.Products
{
	[ TestFixture ]
	internal class ProductsTests
	{
		private readonly IOpenCartFactory OpenCartFactory = new OpenCartFactory();
		private OpenCartConfig Config;

		[ SetUp ]
		public void Init()
		{
			const string credentialsFilePath = @"..\..\Files\OpenCartCredentials.csv";

			var cc = new CsvContext();
			var testConfig = cc.Read< TestConfig >( credentialsFilePath, new CsvFileDescription { FirstLineHasColumnNames = true } ).FirstOrDefault();

			if( testConfig != null )
				this.Config = new OpenCartConfig( testConfig.ApiKey, testConfig.ShopUrl );
		}

		[ Test ]
		public void GetProducts()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );
			var products = service.GetProducts();

			products.Count().Should().BeGreaterThan( 0 );
		}

		[ Test ]
		public void ProductOptionsQuantityUpdated()
		{
			var service = this.OpenCartFactory.CreateProductsService( this.Config );

			var productToUpdate = new OpenCartProduct { Id = 47, Quantity = 10 };
			service.UpdateProducts( new List< OpenCartProduct > { productToUpdate } );
		}
	}
}