// <auto-generated>
//     Generated by the protocol buffer compiler.  DO NOT EDIT!
//     source: Protos/TheOnly.proto
// </auto-generated>
#pragma warning disable 0414, 1591, 8981, 0612
#region Designer generated code

using grpc = global::Grpc.Core;

namespace TheOnlyGen {
  public static partial class TheOnly
  {
    static readonly string __ServiceName = "the_only_service.TheOnly";

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static void __Helper_SerializeMessage(global::Google.Protobuf.IMessage message, grpc::SerializationContext context)
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (message is global::Google.Protobuf.IBufferMessage)
      {
        context.SetPayloadLength(message.CalculateSize());
        global::Google.Protobuf.MessageExtensions.WriteTo(message, context.GetBufferWriter());
        context.Complete();
        return;
      }
      #endif
      context.Complete(global::Google.Protobuf.MessageExtensions.ToByteArray(message));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static class __Helper_MessageCache<T>
    {
      public static readonly bool IsBufferMessage = global::System.Reflection.IntrospectionExtensions.GetTypeInfo(typeof(global::Google.Protobuf.IBufferMessage)).IsAssignableFrom(typeof(T));
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static T __Helper_DeserializeMessage<T>(grpc::DeserializationContext context, global::Google.Protobuf.MessageParser<T> parser) where T : global::Google.Protobuf.IMessage<T>
    {
      #if !GRPC_DISABLE_PROTOBUF_BUFFER_SERIALIZATION
      if (__Helper_MessageCache<T>.IsBufferMessage)
      {
        return parser.ParseFrom(context.PayloadAsReadOnlySequence());
      }
      #endif
      return parser.ParseFrom(context.PayloadAsNewBuffer());
    }

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetInitialMatrixRequest> __Marshaller_the_only_service_GetInitialMatrixRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetInitialMatrixRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetInitialMatrixReply> __Marshaller_the_only_service_GetInitialMatrixReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetInitialMatrixReply.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetInitialMatrixRowRequest> __Marshaller_the_only_service_GetInitialMatrixRowRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetInitialMatrixRowRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetInitialMatrixColumnRequest> __Marshaller_the_only_service_GetInitialMatrixColumnRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetInitialMatrixColumnRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetTaskRequest> __Marshaller_the_only_service_GetTaskRequest = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetTaskRequest.Parser));
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Marshaller<global::TheOnlyGen.GetTaskReply> __Marshaller_the_only_service_GetTaskReply = grpc::Marshallers.Create(__Helper_SerializeMessage, context => __Helper_DeserializeMessage(context, global::TheOnlyGen.GetTaskReply.Parser));

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TheOnlyGen.GetInitialMatrixRequest, global::TheOnlyGen.GetInitialMatrixReply> __Method_GetInitialMatrix = new grpc::Method<global::TheOnlyGen.GetInitialMatrixRequest, global::TheOnlyGen.GetInitialMatrixReply>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "GetInitialMatrix",
        __Marshaller_the_only_service_GetInitialMatrixRequest,
        __Marshaller_the_only_service_GetInitialMatrixReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TheOnlyGen.GetInitialMatrixRowRequest, global::TheOnlyGen.GetInitialMatrixReply> __Method_GetInitialMatrixRow = new grpc::Method<global::TheOnlyGen.GetInitialMatrixRowRequest, global::TheOnlyGen.GetInitialMatrixReply>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "GetInitialMatrixRow",
        __Marshaller_the_only_service_GetInitialMatrixRowRequest,
        __Marshaller_the_only_service_GetInitialMatrixReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TheOnlyGen.GetInitialMatrixColumnRequest, global::TheOnlyGen.GetInitialMatrixReply> __Method_GetInitialMatrixColumn = new grpc::Method<global::TheOnlyGen.GetInitialMatrixColumnRequest, global::TheOnlyGen.GetInitialMatrixReply>(
        grpc::MethodType.ServerStreaming,
        __ServiceName,
        "GetInitialMatrixColumn",
        __Marshaller_the_only_service_GetInitialMatrixColumnRequest,
        __Marshaller_the_only_service_GetInitialMatrixReply);

    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    static readonly grpc::Method<global::TheOnlyGen.GetTaskRequest, global::TheOnlyGen.GetTaskReply> __Method_GetTask = new grpc::Method<global::TheOnlyGen.GetTaskRequest, global::TheOnlyGen.GetTaskReply>(
        grpc::MethodType.Unary,
        __ServiceName,
        "GetTask",
        __Marshaller_the_only_service_GetTaskRequest,
        __Marshaller_the_only_service_GetTaskReply);

    /// <summary>Service descriptor</summary>
    public static global::Google.Protobuf.Reflection.ServiceDescriptor Descriptor
    {
      get { return global::TheOnlyGen.TheOnlyReflection.Descriptor.Services[0]; }
    }

    /// <summary>Base class for server-side implementations of TheOnly</summary>
    [grpc::BindServiceMethod(typeof(TheOnly), "BindService")]
    public abstract partial class TheOnlyBase
    {
      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task GetInitialMatrix(global::TheOnlyGen.GetInitialMatrixRequest request, grpc::IServerStreamWriter<global::TheOnlyGen.GetInitialMatrixReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task GetInitialMatrixRow(global::TheOnlyGen.GetInitialMatrixRowRequest request, grpc::IServerStreamWriter<global::TheOnlyGen.GetInitialMatrixReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task GetInitialMatrixColumn(global::TheOnlyGen.GetInitialMatrixColumnRequest request, grpc::IServerStreamWriter<global::TheOnlyGen.GetInitialMatrixReply> responseStream, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

      [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
      public virtual global::System.Threading.Tasks.Task<global::TheOnlyGen.GetTaskReply> GetTask(global::TheOnlyGen.GetTaskRequest request, grpc::ServerCallContext context)
      {
        throw new grpc::RpcException(new grpc::Status(grpc::StatusCode.Unimplemented, ""));
      }

    }

    /// <summary>Creates service definition that can be registered with a server</summary>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static grpc::ServerServiceDefinition BindService(TheOnlyBase serviceImpl)
    {
      return grpc::ServerServiceDefinition.CreateBuilder()
          .AddMethod(__Method_GetInitialMatrix, serviceImpl.GetInitialMatrix)
          .AddMethod(__Method_GetInitialMatrixRow, serviceImpl.GetInitialMatrixRow)
          .AddMethod(__Method_GetInitialMatrixColumn, serviceImpl.GetInitialMatrixColumn)
          .AddMethod(__Method_GetTask, serviceImpl.GetTask).Build();
    }

    /// <summary>Register service method with a service binder with or without implementation. Useful when customizing the service binding logic.
    /// Note: this method is part of an experimental API that can change or be removed without any prior notice.</summary>
    /// <param name="serviceBinder">Service methods will be bound by calling <c>AddMethod</c> on this object.</param>
    /// <param name="serviceImpl">An object implementing the server-side handling logic.</param>
    [global::System.CodeDom.Compiler.GeneratedCode("grpc_csharp_plugin", null)]
    public static void BindService(grpc::ServiceBinderBase serviceBinder, TheOnlyBase serviceImpl)
    {
      serviceBinder.AddMethod(__Method_GetInitialMatrix, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::TheOnlyGen.GetInitialMatrixRequest, global::TheOnlyGen.GetInitialMatrixReply>(serviceImpl.GetInitialMatrix));
      serviceBinder.AddMethod(__Method_GetInitialMatrixRow, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::TheOnlyGen.GetInitialMatrixRowRequest, global::TheOnlyGen.GetInitialMatrixReply>(serviceImpl.GetInitialMatrixRow));
      serviceBinder.AddMethod(__Method_GetInitialMatrixColumn, serviceImpl == null ? null : new grpc::ServerStreamingServerMethod<global::TheOnlyGen.GetInitialMatrixColumnRequest, global::TheOnlyGen.GetInitialMatrixReply>(serviceImpl.GetInitialMatrixColumn));
      serviceBinder.AddMethod(__Method_GetTask, serviceImpl == null ? null : new grpc::UnaryServerMethod<global::TheOnlyGen.GetTaskRequest, global::TheOnlyGen.GetTaskReply>(serviceImpl.GetTask));
    }

  }
}
#endregion
