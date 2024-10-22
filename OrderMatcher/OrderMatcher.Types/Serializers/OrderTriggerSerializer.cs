﻿using System;

namespace OrderMatcher.Types.Serializers
{
    public class OrderTriggerSerializer : Serializer
    {
        private static readonly short version;
        private static readonly int messageLengthOffset;
        private static readonly int messageTypeOffset;
        private static readonly int versionOffset;
        private static readonly int orderIdOffset;
        private static readonly int userIdOffset;
        private static readonly int timestampOffset;
        private static readonly int messageSequenceOffset;

        private static readonly int sizeOfMessageLength;
        private static readonly int sizeOfMessage;
        private static readonly int sizeOfVersion;
        private static readonly int sizeOfMessagetType;
        private static readonly int sizeOfOrderId;
        private static readonly int sizeOfUserId;
        private static readonly int sizeOfTimestamp;
        private static readonly int sizeOfMessageSequence;

        public static int MessageSize => sizeOfMessage;

        static OrderTriggerSerializer()
        {
            sizeOfMessageLength = sizeof(int);
            sizeOfVersion = sizeof(short);
            sizeOfMessagetType = sizeof(MessageType);
            sizeOfOrderId = OrderId.SizeOfOrderId;
            sizeOfUserId = UserId.SizeOfUserId;
            sizeOfTimestamp = sizeof(int);
            sizeOfMessageLength = sizeof(int);
            sizeOfMessageSequence = sizeof(long);
            version = 1;

            messageLengthOffset = 0;
            messageTypeOffset = messageLengthOffset + sizeOfMessageLength;
            versionOffset = messageTypeOffset + sizeOfMessagetType;
            orderIdOffset = versionOffset + sizeOfVersion;
            userIdOffset = orderIdOffset + sizeOfOrderId;
            timestampOffset = userIdOffset + sizeOfUserId;
            messageSequenceOffset = timestampOffset + sizeOfTimestamp;
            sizeOfMessage = messageSequenceOffset + sizeOfMessageSequence;
        }

        public static void Serialize(OrderTrigger orderTrigger, Span<byte> bytes)
        {
            if (orderTrigger == null)
            {
                throw new ArgumentNullException(nameof(orderTrigger));
            }

            Serialize(orderTrigger.MessageSequence, orderTrigger.OrderId, orderTrigger.UserId, orderTrigger.Timestamp, bytes);
        }

        public static void Serialize(long messageSequence, OrderId orderId, UserId userId, int timestamp, Span<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length < MessageSize)
                throw new ArgumentException(Constant.INVALID_SIZE, nameof(bytes));

            Write(bytes.Slice(messageLengthOffset, 4), sizeOfMessage);
            bytes[messageTypeOffset] = (byte)MessageType.OrderTrigger;
            Write(bytes[versionOffset..], version);
            OrderId.WriteBytes(bytes[orderIdOffset..], orderId);
            UserId.WriteBytes(bytes[userIdOffset..], userId);
            Write(bytes[timestampOffset..], timestamp);
            Write(bytes[messageSequenceOffset..], messageSequence);
        }

        public static OrderTrigger Deserialize(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != MessageSize)
                throw new Exception("Order Trigger Message must be of Size : " + sizeOfMessage);

            var messageType = (MessageType)(bytes[messageTypeOffset]);

            if (messageType != MessageType.OrderTrigger)
                throw new Exception(Constant.INVALID_MESSAGE);

            var version = BitConverter.ToInt16(bytes.Slice(versionOffset));
            if (version != OrderTriggerSerializer.version)
                throw new Exception(Constant.INVALID_VERSION);

            var orderTrigger = new OrderTrigger();

            orderTrigger.OrderId = OrderId.ReadOrderId(bytes[orderIdOffset..]);
            orderTrigger.UserId = UserId.ReadUserId(bytes[userIdOffset..]);
            orderTrigger.Timestamp = BitConverter.ToInt32(bytes[timestampOffset..]);
            orderTrigger.MessageSequence = BitConverter.ToInt64(bytes[messageSequenceOffset..]);

            return orderTrigger;
        }
    }
}
