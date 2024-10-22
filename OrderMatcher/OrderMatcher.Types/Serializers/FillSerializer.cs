﻿using System;

namespace OrderMatcher.Types.Serializers
{
    public class FillSerializer : Serializer
    {
        private static readonly short version;
        private static readonly int messageLengthOffset;
        private static readonly int messageTypeOffset;
        private static readonly int versionOffset;
        private static readonly int makerOrderIdOffset;
        private static readonly int takerOrderIdOffset;
        private static readonly int makerUserIdOffset;
        private static readonly int takerUserIdOffset;
        private static readonly int matchRateOffset;
        private static readonly int matchQuantityOffset;
        private static readonly int timestampOffset;
        private static readonly int isAskRemainingNullOffset;
        private static readonly int askRemainingQuantityOffset;
        private static readonly int isBidCostNullOffset;
        private static readonly int bidCostOffset;
        private static readonly int isBidFeeNullOffset;
        private static readonly int bidFeeOffset;
        private static readonly int isAskFeeNullOffset;
        private static readonly int askFeeOffset;
        private static readonly int messageSequenceOffset;

        private static readonly int sizeOfMessageLength;
        private static readonly int sizeOfMessage;
        private static readonly int sizeOfVersion;
        private static readonly int sizeOfMessagetType;
        private static readonly int sizeOfMakerOrderId;
        private static readonly int sizeOfTakerOrderId;
        private static readonly int sizeOfMakerUserId;
        private static readonly int sizeOfTakerUserId;
        private static readonly int sizeOfMatchRate;
        private static readonly int sizeOfMatchQuantity;
        private static readonly int sizeOfTimestamp;
        private static readonly int sizeOfAskRemainingQuantity;
        private static readonly int sizeOfAskFee;
        private static readonly int sizeOfBidCost;
        private static readonly int sizeOfBidFee;
        private static readonly int sizeOfIsAskRemainingNull;
        private static readonly int sizeOfIsAskFeeNull;
        private static readonly int sizeOfIsBidCostNull;
        private static readonly int sizeOfIsBidFeeNull;
        private static readonly int sizeOfMessageSequence;

        public static int MessageSize => sizeOfMessage;

        static FillSerializer()
        {
            sizeOfMessageLength = sizeof(int);
            sizeOfVersion = sizeof(short);
            sizeOfMessagetType = sizeof(MessageType);
            sizeOfMakerOrderId = OrderId.SizeOfOrderId;
            sizeOfTakerOrderId = OrderId.SizeOfOrderId;
            sizeOfMakerUserId = UserId.SizeOfUserId;
            sizeOfTakerUserId = UserId.SizeOfUserId;
            sizeOfMatchRate = Price.SizeOfPrice;
            sizeOfMatchQuantity = Quantity.SizeOfQuantity;
            sizeOfAskRemainingQuantity = Quantity.SizeOfQuantity;
            sizeOfBidCost = Quantity.SizeOfQuantity;
            sizeOfTimestamp = sizeof(int);
            sizeOfIsAskRemainingNull = sizeof(bool);
            sizeOfIsAskFeeNull = sizeof(bool);
            sizeOfAskFee = Quantity.SizeOfQuantity;
            sizeOfIsBidFeeNull = sizeof(bool);
            sizeOfBidFee = Quantity.SizeOfQuantity;
            sizeOfIsBidCostNull = sizeof(bool);
            sizeOfMessageSequence = sizeof(long);
            version = 1;

            messageLengthOffset = 0;
            messageTypeOffset = messageLengthOffset + sizeOfMessageLength;
            versionOffset = messageTypeOffset + sizeOfMessagetType;
            makerOrderIdOffset = versionOffset + sizeOfVersion;
            takerOrderIdOffset = makerOrderIdOffset + sizeOfMakerOrderId;
            makerUserIdOffset = takerOrderIdOffset + sizeOfTakerOrderId;
            takerUserIdOffset = makerUserIdOffset + sizeOfMakerUserId;
            matchRateOffset = takerUserIdOffset + sizeOfTakerUserId;
            matchQuantityOffset = matchRateOffset + sizeOfMatchRate;
            isAskRemainingNullOffset = matchQuantityOffset + sizeOfMatchQuantity;
            askRemainingQuantityOffset = isAskRemainingNullOffset + sizeOfIsAskRemainingNull;
            isAskFeeNullOffset = askRemainingQuantityOffset + sizeOfAskRemainingQuantity;
            askFeeOffset = isAskFeeNullOffset + sizeOfIsAskFeeNull;
            isBidCostNullOffset = askFeeOffset + sizeOfAskFee;
            bidCostOffset = isBidCostNullOffset + sizeOfIsBidCostNull;
            isBidFeeNullOffset = bidCostOffset + sizeOfBidCost;
            bidFeeOffset = isBidFeeNullOffset + sizeOfIsBidFeeNull;
            timestampOffset = bidFeeOffset + sizeOfBidFee;
            messageSequenceOffset = timestampOffset + sizeOfTimestamp;
            sizeOfMessage = messageSequenceOffset + sizeOfMessageSequence;
        }

        public static void Serialize(Fill fill, Span<byte> bytes)
        {
            if (fill == null)
                throw new ArgumentNullException(nameof(fill));

            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            Serialize(fill.MessageSequence, fill.MakerOrderId, fill.TakerOrderId, fill.MakerUserId, fill.TakerUserId, fill.MatchRate, fill.MatchQuantity, fill.AskRemainingQuantity, fill.AskFee, fill.BidCost, fill.BidFee, fill.Timestamp, bytes);
        }

        public static void Serialize(long messageSequence, OrderId makerOrderId, OrderId takerOrderId, UserId makerUserId, UserId takerUserId, Price matchRate, Quantity matchQuantity, Quantity? remainingAskQuantiy, Quantity? askFee, Quantity? bidCost, Quantity? bidFee, int timeStamp, Span<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length < sizeOfMessage)
                throw new ArgumentException(Constant.INVALID_SIZE, nameof(bytes));

            Write(bytes[messageLengthOffset..], sizeOfMessage);
            bytes[messageTypeOffset] = (byte)MessageType.Fill;
            Write(bytes[versionOffset..], version);
            OrderId.WriteBytes(bytes[makerOrderIdOffset..], makerOrderId);
            OrderId.WriteBytes(bytes[takerOrderIdOffset..], takerOrderId);
            UserId.WriteBytes(bytes[makerUserIdOffset..], makerUserId);
            UserId.WriteBytes(bytes[takerUserIdOffset..], takerUserId);
            Price.WriteBytes(bytes[matchRateOffset..], matchRate);
            Quantity.WriteBytes(bytes[matchQuantityOffset..], matchQuantity);
            bytes[isAskRemainingNullOffset] = Convert.ToByte(remainingAskQuantiy.HasValue);

            if (remainingAskQuantiy.HasValue)
                Quantity.WriteBytes(bytes[askRemainingQuantityOffset..], remainingAskQuantiy.Value);

            bytes[isAskFeeNullOffset] = Convert.ToByte(askFee.HasValue);

            if (askFee.HasValue)
                Quantity.WriteBytes(bytes[askFeeOffset..], askFee.Value);

            bytes[isBidCostNullOffset] = Convert.ToByte(bidCost.HasValue);

            if (bidCost.HasValue)
                Quantity.WriteBytes(bytes[bidCostOffset..], bidCost.Value);

            bytes[isBidFeeNullOffset] = Convert.ToByte(bidFee.HasValue);

            if (bidFee.HasValue)
                Quantity.WriteBytes(bytes[bidFeeOffset..], bidFee.Value);

            Write(bytes[timestampOffset..], timeStamp);
            Write(bytes[messageSequenceOffset..], messageSequence);
        }

        public static Fill Deserialize(ReadOnlySpan<byte> bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            if (bytes.Length != sizeOfMessage)
                throw new Exception("Fill Message must be of Size : " + sizeOfMessage);

            var messageType = (MessageType)bytes[messageTypeOffset];

            if (messageType != MessageType.Fill)
                throw new Exception(Constant.INVALID_MESSAGE);

            var version = BitConverter.ToInt16(bytes[versionOffset..]);

            if (version != FillSerializer.version)
                throw new Exception(Constant.INVALID_VERSION);

            var fill = new Fill();
            fill.MakerOrderId = OrderId.ReadOrderId(bytes[makerOrderIdOffset..]);
            fill.TakerOrderId = OrderId.ReadOrderId(bytes[takerOrderIdOffset..]);
            fill.MakerUserId = UserId.ReadUserId(bytes[makerUserIdOffset..]);
            fill.TakerUserId = UserId.ReadUserId(bytes[takerUserIdOffset..]);
            fill.MatchRate = Price.ReadPrice(bytes[matchRateOffset..]);
            fill.MatchQuantity = Quantity.ReadQuantity(bytes[matchQuantityOffset..]);
            fill.Timestamp = BitConverter.ToInt32(bytes[timestampOffset..]);
            fill.MessageSequence = BitConverter.ToInt64(bytes[messageSequenceOffset..]);

            if (Convert.ToBoolean(bytes[isAskRemainingNullOffset]))
                fill.AskRemainingQuantity = Quantity.ReadQuantity(bytes[askRemainingQuantityOffset..]);

            if (Convert.ToBoolean(bytes[isAskFeeNullOffset]))
                fill.AskFee = Quantity.ReadQuantity(bytes[askFeeOffset..]);

            if (Convert.ToBoolean(bytes[isBidCostNullOffset]))
                fill.BidCost = Quantity.ReadQuantity(bytes[bidCostOffset..]);

            if (Convert.ToBoolean(bytes[isBidFeeNullOffset]))
                fill.BidFee = Quantity.ReadQuantity(bytes[bidFeeOffset..]);

            return fill;
        }
    }
}
