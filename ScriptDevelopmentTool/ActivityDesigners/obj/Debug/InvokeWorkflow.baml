   M S B A M L   `   `   `  ����e  aSystem.Activities.Presentation, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35wTclr-namespace:System.Activities.Presentation;assembly=System.Activities.PresentationSystem.Activities.Presentation  5      �Yclr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation#System.Activities.Presentation.View  5      �_clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation)System.Activities.Presentation.Converters  5      �Zclr-namespace:System.Activities.Presentation.Model;assembly=System.Activities.Presentation$System.Activities.Presentation.Model  5      $  WEM.AutomationActivity.Designers=   7Infosys.WEM.AutomationActivity.Designers.InvokeWorkflow   5      \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      dsapTclr-namespace:System.Activities.Presentation;assembly=System.Activities.Presentation     5      jsapvYclr-namespace:System.Activities.Presentation.View;assembly=System.Activities.Presentation     5      psapc_clr-namespace:System.Activities.Presentation.Converters;assembly=System.Activities.Presentation     5      lModelZclr-namespace:System.Activities.Presentation.Model;assembly=System.Activities.Presentation     5      @d2http://schemas.microsoft.com/expression/blend/2008     6   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     6�   }�.  8   2System.Activities.Presentation.WorkflowViewElement    Collapsible  . $
��Auto��6@   � UseLayoutRounding . $	��477��6g  $��Center=�6s   � 	Resources 5      M   GSystem.Activities.Presentation.Converters.ArgumentToExpressionConverter !  ArgumentToExpressionConverterK   ESystem.Activities.Presentation.Converters.ModelToObjectValueConverter  ModelToObjectValueConverter  tbMargin�� 5	   
   ResourceDictionary_1��%d   &	        &	      &	 &       5
      61     5      6/   u�5      5,10,0,0q�63   5      .��5      � -   5        $��	Container��5      $
��Auto��6   }�.  $��Stretch=�6>   $��0,0,0,-6q�6\    5   
    �� 
TargetType �� Property �� Value��%�  '��       7  '�� D     7  '�� �     7  '�� �     7  'R�     7  �� 5      " ��6   .�5      ��  � ��8   6+   5      �� 5      " ��6   .�5      ��  � ��8   6+   5      �� 5      " ��6   .�5      ��  � ��8   6+   5      �� 5      " ��6   .�5      ��  � ��8   6+   5      �� 5      " R�6   .�5      ��  � ��8   6+   5      5        � RowDefinitions 5!   
   �� 5"      $
��Auto �6   �� 5#      $
��Auto �6   �� 5$      $
��Auto �6   �� 5%      $
��Auto �6   �� 5&      $
��Auto �6   �� 5'      $
��Auto �6   �� 5(      $��40 �6    � ColumnDefinitions 5*   
   �� 5+      $
��Auto �6   �� 5,      $
��Auto �6   �� 5-      $��* �6   .+�50   
   �� -   5        $��lblServiceUri��50   �   $&�� Workflow metadata repository URI��6   $��0��6;   $��0��6K   $��28��6X   $
��Left=�6d   $	��Top=�6�   �� 51   
   -   5        $��lblScriptCat��51   s   $��Workflow Category��6   $��0��6-   $��1��6>   $��28��6K   $
��Left=�6W   $	��Top=�6�   �� 53   
   -   5        $��lblScriptName��53   g   $��Workflow��6   $��0��6#   $��3��63   $��28��6@   $
��Left=�6L   $	��Top=�6|   �� 54   
   -   5        $��lblScriptParam��54   j   $��	Parameter��6   $��0��6$   $��4��65   $��28��6B   $
��Left=�6N   $	��Top=�6�   �� 55   
   -   5        $��lblParameterValue��55   n   $��Default Value��6   $��0��6(   $��5��69   $��28��6F   $
��Left=�6R   $	��Top=�6�   �� 56   
   -   5        $��txtBoxSvcBaseUri��57      $��0��56      $��1��6    $��23��60   $	��210��57   +   $
��Left=�68    � ToolTip$4 .Enter the service URL of the script repository��6T   �� 58   
   -   5        $��btnFetch��59      $��Fetch��58      $��0��6!   $��2��6.   $��23��6?   $
��Left=�6K   $	��Top=�59   !   $��60��69   6D   �� 5:   
   -	   5        	 ��Name$	 txtCategory��5:   /   $��1��6   $��1��6   $	��210��6D   $��23��6P   $
��Left=�6�   {�.  .��6  ��5;      �� 5<      
 �� ImageSource$B
 </WEM.AutomationActivity.Designers;component/Images/combo.png��6    j� 
AlignmentX$ Right=�6i    j� Stretch$
 None=�6|   .��5=   "   5>      � 5?   
   -
   5         �Name$ PopupCat��5?   -   $��2��6   $��1��6    � AllowsTransparency .  � 	StaysOpen .  ��6Y   ��  �� ElementName$ txtCategory�� �� Path$ 	IsChecked� � PlacementTarget 6�   �� $ txtCategory��.��5@      R� -   5         R�Name$ Tree1��5@      5A      5B      �� 5D   
   -   5        $��cmbWorkflowName��5D   =   $��3��6   $��1��6!   $��23��61   $	��215��6T   $
��Left=�6`   $ �� IsSynchronizedWithCurrentItem$
 TrueY�5F      $ Select Script Name��5H   H   {�.  ��5E      �� $ WorkflowNames� �� Mode$ TwoWay=� �� SelectedItem 5G      �� $ ModelItem.WorkflowName�$ TwoWay=�5H   e   �� 5I   
   -   5        $��cmbWFParams��5I   =   $��4��6   $��1��6!   $��23��61   $	��215��6P   $
��Left=�6\   $
 TrueY�5K      $ Select Parameter��5L   D   ��5J      �� $ 
Parameters�$ TwoWay=�5L   D   �� 5M   
   -   5        $��txtBoxParamVal��5N      $��5��5M      $��1��6    $��23��60   $
��Left=�6<   $	��210��5N   )   $# Enter Default Parameter Value��65   �� 5O   
   -   5        $��btnSave��5P      $��Create Variable��5O      $��6��6+   $��1��68   $��23��6I   $
��Left=�6U   $	��Top=�5P       $	��100��68   $A ;Click to create parameter variable for script to be invoked��5Q      5R      5S      