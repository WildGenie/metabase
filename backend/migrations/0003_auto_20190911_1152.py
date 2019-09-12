# Generated by Django 2.2.5 on 2019-09-11 11:52

import django.core.validators
from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('icon', '0002_auto_20190911_1150'),
    ]

    operations = [
        migrations.AlterField(
            model_name='measurementmethod',
            name='identifier',
            field=models.CharField(max_length=16, unique=True, validators=[django.core.validators.MinLengthValidator(16)]),
        ),
        migrations.AlterField(
            model_name='product',
            name='identifier',
            field=models.CharField(max_length=16, unique=True, validators=[django.core.validators.MinLengthValidator(16)]),
        ),
        migrations.AlterField(
            model_name='user',
            name='identifier',
            field=models.CharField(max_length=16, unique=True, validators=[django.core.validators.MinLengthValidator(16)]),
        ),
    ]
