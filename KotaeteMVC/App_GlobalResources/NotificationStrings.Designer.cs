//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option or rebuild the Visual Studio project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Web.Application.StronglyTypedResourceProxyBuilder", "14.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class NotificationStrings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal NotificationStrings() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Resources.NotificationStrings", global::System.Reflection.Assembly.Load("App_GlobalResources"));
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} liked your answer.
        /// </summary>
        internal static string AnswerLikeNotificationBody {
            get {
                return ResourceManager.GetString("AnswerLikeNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} answered your question.
        /// </summary>
        internal static string AnswerNotificationBody {
            get {
                return ResourceManager.GetString("AnswerNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} started following you.
        /// </summary>
        internal static string BeingFollowedLabel {
            get {
                return ResourceManager.GetString("BeingFollowedLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} commented a question you asked.
        /// </summary>
        internal static string CommentQuestionUserNotificationBody {
            get {
                return ResourceManager.GetString("CommentQuestionUserNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} commented an answer that you also commented.
        /// </summary>
        internal static string CommentUserAlsoCommentedNotificationBody {
            get {
                return ResourceManager.GetString("CommentUserAlsoCommentedNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} commented your answer.
        /// </summary>
        internal static string CommentUserAnswerNotificationBody {
            get {
                return ResourceManager.GetString("CommentUserAnswerNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You started following {0}.
        /// </summary>
        internal static string FollowedByLabel {
            get {
                return ResourceManager.GetString("FollowedByLabel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} answered {1}&apos;s question.
        /// </summary>
        internal static string FriendAnswerNotificationBody {
            get {
                return ResourceManager.GetString("FriendAnswerNotificationBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} started following {1}.
        /// </summary>
        internal static string FriendRelationshipBody {
            get {
                return ResourceManager.GetString("FriendRelationshipBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Notifications.
        /// </summary>
        internal static string NotificationsTitle {
            get {
                return ResourceManager.GetString("NotificationsTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your recent notifications.
        /// </summary>
        internal static string NotifictionsHeader {
            get {
                return ResourceManager.GetString("NotifictionsHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} asked you a question.
        /// </summary>
        internal static string QuestionNotificationBody {
            get {
                return ResourceManager.GetString("QuestionNotificationBody", resourceCulture);
            }
        }
    }
}
