﻿using System;

namespace OrderMatcher.Types.Serializers
{
    public class CancelledOrderSerializer : Serializer
    {
        private static readonly short version;
        private static readonly int messageLengthOffset;
        private static readonly int messageTypeOffset;
        private static readonly int versionOffset;
        private static readonly int orderIdOffset;
        private static readonly int userIdOffeset;
        private static readonly int remainingQuantityOffset;
        private static readonly int costOffset;
        private static readonly int feeOffset;
        private static readonly int cancelReasonOffset;
        private static readonly int timestampOffset;
        private static readonly int messageSequenceOffset;

        private static readonly int sizeOfMessageLength;
        private static readonly int sizeOfMessage;
        private static readonly int sizeOfVersion;
        private static readonly int sizeOfMessagetType;
        private static readonly int sizeOfOrderId;
        private static readonly int sizeOfUserId;
        private static readonly int sizeOfRemainingQuantity;
        private static readonly int sizeOfCost;
        private static readonly int sizeOfFee;
        private static readonly int sizeOfCancelReason;
        private static readonly int sizeOfTimestamp;
        private static readonly int sizeOfMessageSequence;

        public static int MessageSize => sizeOfMessage;

        static CancelledOrderSerializer()
        {
            sizeOfMessageLength = sizeof(int);
            sizeOfVersion = sizeof(short);
            sizeOfMessagetType = sizeof(MessageType);
            sizeOfOrderId = OrderId.SizeOfOrderId;
            sizeOfUserId = UserId.SizeOfUserId;
            sizeOfRemainingQuantity = Quantity.SizeOfQuantity;
            sizeOfCost = Quantity.SizeOfQuantity;
            sizeOfFee = Quantity.SizeOfQuantity;
            sizeOfCancelReason = sizeof(CancelReason);
            sizeOfTimestamp = sizeof(int);
            sizeOfMessageSequence = sizeof(long);
            version = 1;

            messageLengthOffset = 0;
            messageTypeOffset = messageLengthOffset + sizeOfMessageLength;
            versionOffset = messageTypeOffset + sizeOfMessagetType;
            orderIdOffset = versionOffset + sizeOfVersion;
            userIdOffeset = orderIdOffset + sizeOfOrderId;
            remainingQuantityOffset = userIdOffeset + sizeOfUserId;
            costOffset = remainingQuantityOffset + sizeOfRemainingQuantity;
            feeOffset = costOffset + sizeOfCost;
            cancelReasonOffset = feeOffset + sizeOfFee;
            timestampOffset = cancelReasonOffset + sizeOfCancelReason;
            messageSequenceOffset = timestampOffset + sizeOfTimestamp;
            sizeOfMessage = messageSequenceOffset + sizeOfMessageSequence;
        }

        public static void Serialize(CancelledOrder cancelledOrder, Span<byte> bytes)
        {
            if (cancelledOrder == null)
                throw new ArgumentNullException(nameof(cancelledOrder));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            Serialize(cancelledOrder.MessageSequence, cancelledOrder.OrderId, cancelledOrder.UserId, cancelledOrder.RemainingQuantity, cancelledOrder.Cost, cancelledOrder.Fee, cancelledOrder.CancelReason, cancelledOrder.Timestamp, bytes);
        }

        public static void Serialize(long messageSequence, OrderId orderId, UserId userId, Quantity remainingQuantity, Quantity cost, Quantity fee, CancelReason cancelReason, int timeStamp, Span<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length < sizeOfMessage)
                throw new ArgumentException(Constant.INVALID_SIZE, nameof(bytes));

            Write(bytes[messageLengthOffset..], sizeOfMessage);
            bytes[messageTypeOffset] = (byte)MessageType.Cancel;
            Write(bytes[versionOffset..], version);
            OrderId.WriteBytes(bytes[orderIdOffset..], orderId);
            UserId.WriteBytes(bytes[userIdOffeset..], userId);
            Quantity.WriteBytes(bytes[remainingQuantityOffset..], remainingQuantity);
            bytes[cancelReasonOffset] = (byte)cancelReason;
            Write(bytes[timestampOffset..], timeStamp);
            Quantity.WriteBytes(bytes[costOffset..], cost);
            Quantity.WriteBytes(bytes[feeOffset..], fee);
            Write(bytes[messageSequenceOffset..], messageSequence);
        }

        public static CancelledOrder Deserialize(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != sizeOfMessage)
                throw new Exception("Canceled Order Message must be of Size : " + sizeOfMessage);

            var messageType = (MessageType)(bytes[messageTypeOffset]);

            if (messageType != MessageType.Cancel)
                throw new Exception(Constant.INVALID_MESSAGE);

            var version = BitConverter.ToInt16(bytes[versionOffset..]);

            if (version != CancelledOrderSerializer.version)
                throw new Exception(Constant.INVALID_VERSION);

            var cancelledOrder = new CancelledOrder();

            cancelledOrder.OrderId = OrderId.ReadOrderId(bytes[orderIdOffset..]);
            cancelledOrder.UserId = UserId.ReadUserId(bytes[userIdOffeset..]);
            cancelledOrder.RemainingQuantity = Quantity.ReadQuantity(bytes[remainingQuantityOffset..]);
            cancelledOrder.CancelReason = (CancelReason)bytes[cancelReasonOffset];
            cancelledOrder.Timestamp = BitConverter.ToInt32(bytes[timestampOffset..]);
            cancelledOrder.Cost = Quantity.ReadQuantity(bytes[costOffset..]);
            cancelledOrder.Fee = Quantity.ReadQuantity(bytes[feeOffset..]);
            cancelledOrder.MessageSequence = BitConverter.ToInt64(bytes[messageSequenceOffset..]);

            return cancelledOrder;
        }
    }
}
