CREATE TABLE [dbo].[ChatUsers] (
    [UserId] NVARCHAR (450) NOT NULL,
    [ChatId] INT            NOT NULL,
    CONSTRAINT [PK_ChatUsers] PRIMARY KEY CLUSTERED ([ChatId] ASC, [UserId] ASC),
    CONSTRAINT [FK_ChatUsers_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ChatUsers_Chats_ChatId] FOREIGN KEY ([ChatId]) REFERENCES [dbo].[Chats] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_ChatUsers_UserId]
    ON [dbo].[ChatUsers]([UserId] ASC);

