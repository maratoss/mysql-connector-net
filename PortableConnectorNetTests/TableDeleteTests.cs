﻿// Copyright © 2015, Oracle and/or its affiliates. All rights reserved.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using MySql.XDevAPI.Relational;
using Xunit;

namespace PortableConnectorNetTests
{
  public class TableDeleteTests : IClassFixture<TableFixture>
  {
    TableFixture fixture;

    public TableDeleteTests(TableFixture fixture)
    {
      this.fixture = fixture;

      Assert.True(fixture.GetNodeSession().ExecuteSql("DELETE FROM " + fixture.TableInsert).Succeeded);

      //inserts data
      var tableInsert = fixture.GetTableInsert();
      var insertStatement = tableInsert.Insert();
      int rowsToInsert = 10;
      for (int i = 1; i <= rowsToInsert; i++)
      {
        insertStatement.Values(i, i, i);
      }
      Assert.True(insertStatement.Execute().Succeeded);

      Assert.Equal(rowsToInsert, CountRows());
    }

    private int CountRows()
    {
      var result = fixture.GetTableInsert().Select().Execute();
      Assert.True(result.Succeeded);
      while (result.Next());
      return result.Rows.Count;
    }

    private void ExecuteDelete(TableDeleteStatement statement, int expectedRowsCount)
    {
      var table = fixture.GetTableInsert();
      var result = statement.Execute();
      Assert.True(result.Succeeded);
      Assert.Equal(expectedRowsCount, CountRows());
    }

    [Fact]
    public void DeleteAllTest()
    {
      ExecuteDelete(fixture.GetTableInsert().Delete(), 0);
    }

    [Fact]
    public void DeleteConditionTest()
    {
      ExecuteDelete(fixture.GetTableInsert().Delete().Where("age % 2 = 0"), 5);
    }

    [Fact]
    public void DeleteOrderbyAndLimit()
    {
      ExecuteDelete(fixture.GetTableInsert().Delete().OrderBy("age Desc").Limit(3), 7);
    }

    [Fact]
    public void DeleteConditionOrderbyAndLimit()
    {
      ExecuteDelete(fixture.GetTableInsert().Delete().Where("employee_id > 5").OrderBy("employee_id Desc").Limit(2), 8);
    }
  }
}
