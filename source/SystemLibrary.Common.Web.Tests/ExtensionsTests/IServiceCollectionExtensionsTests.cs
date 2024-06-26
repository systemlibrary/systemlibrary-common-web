﻿using System.Collections;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Web.Tests;

[TestClass]
public class IServiceCollectionExtensionsTests
{
    [TestMethod]
    public void Create_Data_Protection_File()
    {
        var options = new ServicesCollectionOptions();

        options.UseAutomaticKeyGenerationFile = true;

        var service = new ServiceCollectionTest();

        service.UseAutomaticKeyGenerationFile(options);
    }

    [TestMethod]
    public void Do_Not_Create_Data_Protection_File()
    {
        var options = new ServicesCollectionOptions();

        options.UseAutomaticKeyGenerationFile = false;

        var service = new ServiceCollectionTest();

        service.UseAutomaticKeyGenerationFile(options);
    }
}


public class ServiceCollectionTest : IServiceCollection
{
    public ServiceDescriptor this[int index] { get { return null; } set { } }

    public int Count { get; }
    public bool IsReadOnly { get; }

    public void Add(ServiceDescriptor item)
    {
    }

    public void Clear()
    {
    }

    public bool Contains(ServiceDescriptor item)
    {
        return false;
    }

    public void CopyTo(ServiceDescriptor[] array, int arrayIndex)
    {
    }

    public IEnumerator<ServiceDescriptor> GetEnumerator()
    {
        return null;
    }

    public int IndexOf(ServiceDescriptor item)
    {
        return 0;
    }

    public void Insert(int index, ServiceDescriptor item)
    {
    }

    public bool Remove(ServiceDescriptor item)
    {
        return true;
    }

    public void RemoveAt(int index)
    {
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new System.NotImplementedException();
    }
}